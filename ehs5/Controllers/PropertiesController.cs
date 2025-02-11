using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ehs5.Models;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;

namespace ehs5.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly Ehs1Context _context;

        public PropertiesController(Ehs1Context context)
        {
            _context = context;
        }
        private readonly string _storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=chiggy;AccountKey=GTvptVJ1ThRpkNwEm7KdH+o3Ude+DNEYVo0pFvrP+T3fKIsnIvCggSZPkdqR+qGaMCcyJqM7lbkT+ASthPcdsQ==;EndpointSuffix=core.windows.net";
        private readonly string _containerName = "container";

        // GET: Properties
        public IActionResult Index(int? cityId, string sortOrder)
        {
            var properties = _context.Properties.Include(p => p.Seller).AsQueryable();

            if (cityId.HasValue)
            {
                properties = properties.Where(p => p.CityId == cityId);
            }

            if (sortOrder == "price_asc")
            {
                properties = properties.OrderBy(p => p.PriceRange);
            }
            else if (sortOrder == "price_desc")
            {
                properties = properties.OrderByDescending(p => p.PriceRange);
            }

            ViewData["Cities"] = _context.Cities.ToList(); // Pass list of cities
            return View(properties.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int buyerId, int propertyId)
        {
            try
            {
                var existingCartItem = await _context.Carts
                    .FirstOrDefaultAsync(c => c.BuyerId == buyerId && c.PropertyId == propertyId);

                if (existingCartItem != null)
                {
                    return Json(new { success = false, message = "Already Added" });
                }

                var newCartItem = new Cart
                {
                    BuyerId = buyerId,
                    PropertyId = propertyId
                };

                _context.Carts.Add(newCartItem);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Added to Cart" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while adding the item to the cart." });
            }
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .Include(a => a.Seller)
                .FirstOrDefaultAsync(m => m.PropertyId == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // GET: Properties/Create
        [JWTAction(allowedRoles: "Admin,Seller")]
        public async Task<IActionResult> Create()
        {
            var sellers = await _context.Sellers.ToListAsync();
            var cities = await _context.Cities.ToListAsync();

            // Use 'UserName' instead of 'FirstName'
            ViewData["Sellers"] = new SelectList(sellers, "SellerId", "UserName");
            ViewData["Cities"] = new SelectList(cities, "CityId", "CityName");

            return View();
        }

        // POST: Properties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [JWTAction(allowedRoles: "Admin,Seller")]
        public async Task<IActionResult> Create([Bind("PropertyId,PropertyName,PropertyType,PropertyOption,Description,Address,PriceRange,InitialDeposit,Landmark,IsActive,SellerId,CityId")] Property property, IFormFile[] images)
        {
            if (!ModelState.IsValid)
            {
                // Set property as active by default if not specified
                property.IsActive = true;

                _context.Add(property);
                await _context.SaveChangesAsync();

                // Handle image uploads
                if (images != null && images.Length > 0)
                {


                    await UploadImagesAsync(property.PropertyId, images);
                    
                    var authJSON = new
                    {
                        
                            propertyId = property.PropertyId,
                            propertyName = property.PropertyName,
                            propertyType = property.PropertyType,
                            propertyOption = property.PropertyOption,
                            description = property.Description,
                            address = property.Address,
                            priceRange = property.PriceRange,
                            initialDeposit = property.InitialDeposit,
                            landmark = property.Landmark,
                            isActive = property.IsActive,
                            sellerId = property.SellerId,
                            cityId = property.CityId
                        
                    };

                    string json = JsonSerializer.Serialize(authJSON);


                    //using (var client = new HttpClient())
                    //{
                    //    client.Timeout = TimeSpan.FromSeconds(30);
                    //    var validateUrl = "https://prod-16.northcentralus.logic.azure.com:443/workflows/06899f5ea0334efe992f760539086155/triggers/When_a_HTTP_request_is_received/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2FWhen_a_HTTP_request_is_received%2Frun&sv=1.0&sig=ddYsy9ydNWqNOAgjmqsRrLTAz8NQYECCeatXm23AyKw";

                    //    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    //    var response = await client.PostAsync(validateUrl, content);
                    //};


                    using (var client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(60);  // Increased timeout to 60 seconds for longer processing times

                        var validateUrl = "https://prod-16.northcentralus.logic.azure.com:443/workflows/06899f5ea0334efe992f760539086155/triggers/When_a_HTTP_request_is_received/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2FWhen_a_HTTP_request_is_received%2Frun&sv=1.0&sig=ddYsy9ydNWqNOAgjmqsRrLTAz8NQYECCeatXm23AyKw";

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        try
                        {
                            var response = await client.PostAsync(validateUrl, content);

                            if (response.IsSuccessStatusCode)
                            {
                                // Successfully triggered Logic App workflow
                                var responseContent = await response.Content.ReadAsStringAsync();
                                Console.WriteLine("Logic App Triggered Successfully: " + responseContent);
                            }
                            else
                            {
                                // Log the failure status and content for debugging
                                var responseContent = await response.Content.ReadAsStringAsync();
                                Console.WriteLine($"Failed to trigger Logic App. Status Code: {response.StatusCode}, Response: {responseContent}");
                            }
                        }
                        catch (HttpRequestException ex)
                        {
                            // Handle errors related to HTTP requests (e.g., network issues)
                            Console.WriteLine($"Request Error: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            // Handle any other errors
                            Console.WriteLine($"General Error: {ex.Message}");
                        }
                    }


                    foreach (var image in images)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await image.CopyToAsync(memoryStream);
                            var propertyImage = new Image
                            {
                                PropertyId = property.PropertyId,
                                Image1 = memoryStream.ToArray() // Store image byte array
                            };
                            _context.Images.Add(propertyImage);
                            await _context.SaveChangesAsync();
                        }
                    }


                    // return RedirectToAction(nameof(Index));

                }
                    // Rebind dropdowns if validation fails
                   
                }
            ViewData["Sellers"] = new SelectList(_context.Sellers, "SellerId", "UserName", property.SellerId);
            ViewData["Cities"] = new SelectList(_context.Cities, "CityId", "CityName", property.CityId);
            return View(property);
        }


        // GET: Properties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Property property, List<IFormFile> Images)
        {
            if (id != property.PropertyId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(property);
            }

            try
            {
                // Handle file uploads
                if (Images != null && Images.Count > 0)
                {
                    foreach (var file in Images)
                    {
                        if (file.Length > 0)
                        {
                            var filePath = Path.Combine("wwwroot/uploads", file.FileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                          var image = new Image // ✅ Use Image instead of PropertyImage
{
    PropertyId = property.PropertyId, // Make sure this field exists in Image
    ImageUrl = "/uploads/" + file.FileName
};
property.Images.Add(image);

                        }
                    }
                }

                _context.Update(property);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Properties.Any(e => e.PropertyId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }




        // POST: Properties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropertyId,PropertyName,PropertyType,PropertyOption,Description,Address,PriceRange,InitialDeposit,Landmark,IsActive,SellerId,CityId")] Property @property)
        {
            if (id != @property.PropertyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@property);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(@property.PropertyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CityId"] = new SelectList(await _context.Cities.ToListAsync(), "CityId", "CityName", @property.CityId);
            ViewData["SellerId"] = new SelectList(_context.Sellers, "SellerId", "UserName", @property.SellerId);
            return View(@property);
        }

        // GET: Properties/Delete/5
        [JWTAction(allowedRoles: "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Properties
                .Include(a => a.Seller)
                .FirstOrDefaultAsync(m => m.PropertyId == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [JWTAction(allowedRoles: "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @property = await _context.Properties.FindAsync(id);
            if (@property != null)
            {
                _context.Properties.Remove(@property);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Properties/Verified/5
        // GET: Properties/Verified
        [JWTAction(allowedRoles: "Admin")]
        public async Task<IActionResult> Verified()
        {
            // Fetch all verified properties (no sellerId filter)
            var verifiedProperties = await _context.Properties
                .Where(p => p.IsVerified)  // Only filter by IsVerified
                .Include(p => p.Seller)  // Include Seller info for display
                .ToListAsync();

            // Return the view with the filtered list of verified properties
            return View(verifiedProperties);
        }



        // GET: Properties/Deactivated
        public async Task<IActionResult> Deactivated()
        {
            // Fetch all deactivated properties (IsActive = false)
            var deactivatedProperties = await _context.Properties
                .Where(p => !p.IsActive)  // Filter by IsActive == false (deactivated)
                .Include(p => p.Seller)  // Include Seller info for display
                .ToListAsync();

            // If no deactivated properties are found, return a custom view or message
            if (!deactivatedProperties.Any())
            {
                return View("NoPropertiesFound");  // Custom view in case no deactivated properties are found
            }

            // Return the deactivated properties view
            return View(deactivatedProperties);
        }


        // GET: Properties/Activated
        public async Task<IActionResult> Activated()
        {
            var activatedProperties = await _context.Properties
                .Where(p => p.IsActive)
                .Include(p => p.Seller)
                .ToListAsync();

            return View(activatedProperties);
        }

        // GET: Properties/GetOwnerContactDetails/5
        public async Task<IActionResult> GetOwnerContactDetails(int propertyId)
        {
            var property = await _context.Properties
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId);

            if (property == null || property.Seller == null)
            {
                return Json(new { success = false, message = "Property or Seller not found." });
            }

            var contactDetails = new
            {
                Email = property.Seller.EmailId ?? "Email not available",
                PhoneNumber = property.Seller.PhoneNo ?? "Phone not available"
            };

            return Json(new { success = true, data = contactDetails });
        }

        private async Task UploadImagesAsync(int propertyId, IFormFile[] images)
        {
            if (images == null || images.Length == 0) return;

            BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    string fileName = $"{Guid.NewGuid()}_{image.FileName}";
                    BlobClient blobClient = containerClient.GetBlobClient(fileName);
                    using (var stream = image.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = image.ContentType });
                    }
                    var propertyImage = new PropertyImage { PropertyId = propertyId, ImageUrl = blobClient.Uri.ToString() };
                    _context.PropertyImages.Add(propertyImage);
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task DeleteImagesAsync(IEnumerable<PropertyImage> images)
        {
            if (images == null) return;

            BlobServiceClient blobServiceClient = new BlobServiceClient(_storageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            foreach (var image in images)
            {
                BlobClient blobClient = containerClient.GetBlobClient(new Uri(image.ImageUrl).Segments.Last());
                await blobClient.DeleteIfExistsAsync();
                _context.PropertyImages.Remove(image);
            }
            await _context.SaveChangesAsync();
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.PropertyId == id);
        }
    }
}
