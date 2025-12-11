using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using CMCS.Areas.Identity.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using iText.Commons.Actions.Contexts;

namespace CMCS.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        //constructor to initialize dependencies through dependency injection
        public ClaimsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager; //assign user manager instance to the private field
            _context = context; //assign the database context instance to the private field
            this.webHostEnvironment = webHostEnvironment; //assign the web hosting environment instance to the private field
        }

        //asynchronous action to display a summary view for a specific claim based on its id
        public async Task<IActionResult> Summary(int id)
        {
            //fetch the claim record from the database using the provided id
            var claim = await _context.Claims.FindAsync(id);

            //check if the claim does not exist and return a error
            if (claim == null)
            {
                return NotFound();
            }
            return View(claim); //return the summary view with the claim data
        }

        //httpget action to render the claims view
        [HttpGet]
        public IActionResult Claims()
        {
            return View(); //return the claims view to the client
        }

        //httppost action to process claims submission with an optional supporting document
        [HttpPost]
        public async Task<IActionResult> Claims(Claims claims, IFormFile supportingDocument)
        {
            //log to console that the claims action is called
            Console.WriteLine("Claims action called");

            //retrieve the current user's id from the claims principal
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //assign the retrieved user id to the claim's userid property
            claims.UserID = id;
            Console.WriteLine($"Assigned UserID: {claims.UserID}");

            //remove validation errors related to userid, claimstatus, and datesubmitted
            ModelState.Remove("UserID");
            claims.ClaimStatus = "Pending";
            var valid = new Validator();
            var validRes = valid.Validate(claims);  //perform validation on the claims object and store the result
            ModelState.Remove("ClaimStatus");

            //check if the validation results are valid
            if (validRes.IsValid)
            {
                //if valid, set the ClaimStatus to "Pending"
                claims.ClaimStatus = "Pending";
            }
            else
            {
                //if not valid, set the ClaimStatus to "Rejected"
                claims.ClaimStatus = "Rejected";
            }

            ModelState.Remove("DateSubmitted");
            ModelState.Remove("RateHour");
            ModelState.Remove("HoursWorked");
            ModelState.Remove("OvertimeHours");
            ModelState.Remove("OvertimeRate");

            //check if the supporting document is provided and not empty
            if (supportingDocument != null && supportingDocument.Length > 0)
            {
                //store the document content in memory stream
                using (var ms = new MemoryStream())
                {
                    await supportingDocument.CopyToAsync(ms);

                    //convert file bytes to a base64 string and generate a short guid
                    var fileBytes = ms.ToArray();
                    string base64String = Convert.ToBase64String(fileBytes).Substring(0, 17);
                    string shortGuid = Guid.NewGuid().ToString().Substring(0, 8).Trim();

                    //generate a unique prefix using submission date, names, and guid
                    string distinctPrefix = $"[{claims.DateSubmitted}]-{claims.FirstName}_{claims.LastName}_{shortGuid}_";

                    //store the generated document name in the claim model
                    claims.SupportingDocuments = distinctPrefix + base64String;
                }
            }
            else
            {
                //set supporting documents to null and log the absence of files
                claims.SupportingDocuments = null;
                Console.WriteLine("No supporting documents provided or file is empty.");
            }

            //remove validation errors related to document fields
            ModelState.Remove("supportingDocument");
            ModelState.Remove("SupportingDocuments");

            //check if the user is not logged in and add an error if so
            if (string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError("UserID", "User must be logged in to submit a claim.");
                return View(claims);
            }

            //check if the model state is valid before proceeding
            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState is valid");

                //assign the current date to datesubmitted and calculate total amount
                claims.DateSubmitted = DateTime.Now;
                claims.TotalAmount = claims.HoursWorked * claims.RatePerHour;

                //add overtime to TotalAmount if applicable
                if (claims.OvertimeHours.HasValue && claims.OvertimeRate.HasValue)
                {
                    claims.TotalAmount += claims.OvertimeHours.Value * claims.OvertimeRate.Value;
                }

                claims.DateSubmitted = DateTime.Now; //set the DateSubmitted to current date and time
                claims.TotalAmount = claims.HoursWorked * claims.RatePerHour; //calculate total amount
                //add the claim to the database and save changes
                _context.Add(claims);
                await _context.SaveChangesAsync();
                Console.WriteLine("Claim saved successfully");

                //check if a valid supporting document was uploaded
                if (supportingDocument != null && supportingDocument.Length > 0)
                {
                    Console.WriteLine($"File '{supportingDocument.FileName}' detected. Size: {supportingDocument.Length} bytes.");

                    //define permitted file extensions and validate the file type
                    var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".docx", ".xlsx", ".pdf" };
                    var extension = Path.GetExtension(supportingDocument.FileName).ToLowerInvariant();

                    //check if the extension is invalid and return an error if so
                    if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                    {
                        Console.WriteLine("Invalid file type detected.");
                        ModelState.AddModelError("", "Invalid file type.");
                        return View(claims);
                    }

                    //define permitted mime types and validate the mime type
                    var mimeType = supportingDocument.ContentType;
                    var permittedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };

                    //check if the mime type is invalid and return an error if so
                    if (!permittedMimeTypes.Contains(mimeType))
                    {
                        Console.WriteLine("Invalid MIME type detected.");
                        ModelState.AddModelError("", "Invalid MIME type.");
                        return View(claims);
                    }

                    //define the path for storing uploaded files
                    var uploadsFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "uploads");

                    //create the uploads directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                        Console.WriteLine("Uploads directory created.");
                    }

                    //generate a unique file name and define the file path
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(supportingDocument.FileName);
                    var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                    //save the uploaded file to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await supportingDocument.CopyToAsync(stream);
                        Console.WriteLine("File saved successfully to disk.");
                    }

                    //create a new files object and populate it with file metadata
                    var files = new Files
                    {
                        FileName = uniqueFileName,
                        Length = supportingDocument.Length,
                        ContentType = mimeType,
                        Data = System.IO.File.ReadAllBytes(filePath),
                        ClaimId = claims.Id
                    };

                    //add the file object to the database and save changes
                    _context.Files.Add(files);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("File model added to the database.");
                }
                else
                {
                    //set the supportingdocuments property to an empty string if no file is uploaded
                    claims.SupportingDocuments = "";
                    Console.WriteLine("No supporting documents provided or file is empty.");
                }

                return RedirectToAction("Summary", new { id = claims.Id }); //redirect to the summary view with the newly submitted claim id
            }
            else
            {
                //log to console that the modelstate is invalid and display validation errors
                Console.WriteLine("ModelState is invalid");
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                    }
                }
            }
            return View(claims); //return the view with the claim model in case of validation failure
        }


        //action to display a confirmation view after a claim is submitted
        public IActionResult ClaimSubmitted()
        {
            return View(); //return the claimsubmitted view
        }

        //asynchronous action to list all claims from the database
        public async Task<IActionResult> List()
        {
            //retrieve all claims from the database asynchronously
            var claims = await _context.Claims.ToListAsync();
            return View(claims); //pass the claims list to the view for rendering
        }

        //asynchronous action to view the history of claims submitted by the logged-in user
        public async Task<IActionResult> ViewHistory()
        {
            try
            {
                //get the current user's id from the claims principal
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"Logged in User ID: {userId}");

                //retrieve the user's claims from the database, including associated files
                var claims = await _context.Claims
                    .Include(c => c.File)
                    .Where(c => c.UserID == userId)
                    .ToListAsync();

                //check if no claims were found and log a message if the list is empty
                if (!claims.Any())
                {
                    Console.WriteLine("No claims found for this user.");
                }
                else
                {
                    //log the total number of claims found for the user
                    Console.WriteLine($"Number of claims found: {claims.Count}");

                    //iterate through the claims and log their details
                    foreach (var claim in claims)
                    {
                        Console.WriteLine($"Claim ID: {claim.Id}, Total Files: {claim.File.Count}");

                        //iterate through the files associated with each claim and log their details
                        foreach (var file in claim.File)
                        {
                            Console.WriteLine($"File ID: {file.FileId}, File Name: {file.FileName}");
                        }
                    }
                }
                return View(claims); //return the view with the user's claims history
            }
            catch (Exception ex)
            {
                //log the error message if an exception occurs
                Console.WriteLine($"An error occurred: {ex.Message}");
                return View("Error"); //return the error view in case of exceptions
            }
        }


        //asynchronous action to approve a claim by its id
        public async Task<IActionResult> Approve(int id)
        {
            //log to the console that the Approve method has been invoked for the given claim ID.
            Console.WriteLine($"Approve method invoked for Claim ID: {id}");

            //find the claim by id in the database
            var claims = _context.Claims.Find(id);

            //check if the claim does not exist and show an error message
            if (claims == null)
            {
                Console.WriteLine($"Claim with ID {id} not found."); //log that the claim was not found
                TempData["Message"] = "Claim not found"; //set an error message for the user
                TempData["MessageType"] = "error"; //indicate that the message type is an error
                return RedirectToAction("Index"); //redirect to the index page
            }

            //log claim was found, along with ID and current status
            Console.WriteLine($"Claim found: ID = {claims.Id}, Status = {claims.ClaimStatus}");

            //update the claim status to 'Approve'
            claims.ClaimStatus = "Approved";

            //save changes to the database
            _context.SaveChanges();

            Console.WriteLine($"Claim ID {claims.Id} has been approved."); //claim has been successfully approved.

            //set a success message to notify the user about the approval
            TempData["Message"] = "Claim has been approved";
            TempData["MessageType"] = "success"; //indicate the message is a success
            return RedirectToAction("List"); //redirect to the list of claims
        }

        //asynchronous action to reject a claim by its id
        public async Task<IActionResult> Reject(int id)
        {
            //log console that Reject method is invoked for the given claim ID
            Console.WriteLine($"Reject method invoked for Claim ID: {id}");

            //find the claim by id in the database
            var claims = _context.Claims.Find(id);

            //check if the claim does not exist and show an error message
            if (claims == null)
            {
                TempData["Message"] = "Claim not found"; //set an error message for the user
                TempData["MessageType"] = "error"; //indicate that the message type is an error
                return RedirectToAction("Index"); //redirect to the index page
            }

            //log claim was found, along with ID and current status
            Console.WriteLine($"Claim found: ID = {claims.Id}, Status = {claims.ClaimStatus}");

            claims.ClaimStatus = "Rejected"; //update the claim status to 'Rejected'
            _context.SaveChanges(); //save changes to the database

            //set a success message to notify the user about the rejection
            TempData["Message"] = "Your claim has been rejected - contact HR";
            TempData["MessageType"] = "success"; //indicate the message is a success
            return RedirectToAction("List"); //redirect to the list of claims
        }

        //asynchronous action to display all claims for updating their status
        public async Task<IActionResult> UpdateClaimStatus()
        {
            //retrieve all claims from the database asynchronously
            var claims = await _context.Claims.ToListAsync();

            return View(claims); //pass the claims to the view for rendering
        }


        //http post action to update the status of a claim based on its id
        [HttpPost]
        public async Task<IActionResult> UpdateClaimStatus(int claimId, string newStatus)
        {
            try
            {
                //log the attempt to update the claim status
                Console.WriteLine($"Attempting to update status. Claim ID: {claimId}, New Status: {newStatus}");

                //check if the new status is not null or empty
                if (!string.IsNullOrEmpty(newStatus))
                {
                    //find the claim by its id asynchronously
                    var claim = await _context.Claims.FindAsync(claimId);
                    if (claim != null)
                    {
                        //update the claim's status
                        claim.ClaimStatus = newStatus;

                        //mark the claim as updated in the context
                        _context.Update(claim);

                        //save changes to the database asynchronously
                        await _context.SaveChangesAsync();

                        //log the successful update of the claim status
                        Console.WriteLine($"Status updated to: {claim.ClaimStatus}");

                        //return a success response in JSON format
                        return Json(new { success = true, message = "Status updated successfully." });
                    }
                    else
                    {
                        //log if the claim is not found
                        Console.WriteLine($"Claim not found with ID: {claimId}");
                    }
                }
                else
                {
                    //log if the new status is null or empty
                    Console.WriteLine("New status is null or empty.");
                }
            }
            catch (Exception ex)
            {
                //log the error message and stack trace in case of exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            //return a failure response in JSON format if the update was unsuccessful
            return Json(new { success = false, message = "Failed to update status. Please try again." });
        }

        public async Task<IActionResult> FileDownload(int id)
        {
            //find the file by its id asynchronously
            var file = await _context.Files.FirstOrDefaultAsync(f => f.ClaimId == id);

            //check if the file is not found and return a not found response
            if (file == null)
            {
                return NotFound();
            }

            //return the file as a downloadable response with the appropriate content type and filename
            return File(file.Data, file.ContentType, file.FileName);
        }

        [HttpGet]
        public IActionResult GenReport(string lecturerId)
        {
            ViewBag.LecturerId = lecturerId;
            return View();
        }

        [HttpPost]
        public IActionResult GenReport(string lecturerId, DateTime startDate, DateTime endDate)
        {
            // Fetch all claims for the specific lecturer within the date range
            var claims = _context.Claims
                .Where(c => c.UserID == lecturerId && c.DateSubmitted >= startDate && c.DateSubmitted <= endDate)
                .ToList();

            if (!claims.Any())
            {
                TempData["ErrorMessage"] = "No claims found within the specified date range.";
                return RedirectToAction("List");
            }

            // Separate claims by status
            var approvedClaims = claims.Where(c => c.ClaimStatus == "Approved").ToList();
            var otherStatusClaims = claims.Where(c => c.ClaimStatus != "Approved").ToList();

            // Generate PDF report
            var pdfBytes = GeneratePdf(approvedClaims, otherStatusClaims, startDate, endDate);

            // Return the PDF file
            return File(pdfBytes, "application/pdf", $"claim{startDate:yyyyMMdd}-{endDate:yyyyMMdd}.pdf");
        }

        public async Task<IActionResult> Lecturers()
        {
            var users = await _userManager.Users.ToListAsync(); //fetching all users

            var lecturers = new List<IdentityUser>(); //list to store lecturers

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); //fetch roles of each user
                if (!roles.Contains("Admin")) //exclude users with the "Admin" role
                {
                    lecturers.Add(user); //add non-admin users to lecturers list
                }
            }

            return View(lecturers); //pass the filtered list of users to the view
        }

        //private double calculateTax(double taxInc)
        //{
        //    if (taxInc <= 237100)
        //        return taxInc * 0.18;
        //    else if (taxInc <= 370500)
        //        return 42678 + (taxInc - 237100) * 0.26;
        //    else if (taxInc <= 512800)
        //        return 77362 + (taxInc - 370500) * 0.31;
        //    else if (taxInc <= 673000)
        //        return 121475 + (taxInc - 512800) * 0.36;
        //    else if (taxInc <= 857900)
        //        return 179147 + (taxInc - 673000) * 0.39;
        //    else if (taxInc <= 1817000)
        //        return 251258 + (taxInc - 857900) * 0.41;
        //    else
        //        return 644489 + (taxInc - 1817000) * 0.45;
        //}

        private double CalculateTax(double taxInc)
        {
            // List of tuples to store the tax brackets and corresponding base and rate values
            var taxBrackets = new (double UpperLimit, double BaseTax, double Rate)[]
            {
        (237100, 0, 0.18),
        (370500, 42678, 0.26),
        (512800, 77362, 0.31),
        (673000, 121475, 0.36),
        (857900, 179147, 0.39),
        (1817000, 251258, 0.41),
        (double.MaxValue, 644489, 0.45)
            };

            // Find the appropriate tax bracket based on income
            foreach (var bracket in taxBrackets)
            {
                if (taxInc <= bracket.UpperLimit)
                {
                    return bracket.BaseTax + (taxInc - (bracket.UpperLimit - (bracket.BaseTax == 0 ? 0 : bracket.UpperLimit))) * bracket.Rate;
                }
            }

            // This line should never be reached since all incomes fall within a bracket
            throw new InvalidOperationException("Taxable income does not fit within any tax bracket.");
        }

        private byte[] GeneratePdf(IEnumerable<Claims> paidClaims, IEnumerable<Claims> pendingOrRejectedClaims, DateTime startDate, DateTime endDate)
        {
            // Calculate totals for paid claims
            var totalGrossPay = paidClaims.Sum(c => c.TotalAmount);
            var totalTaxDeductions = paidClaims.Sum(c => CalculateTax(c.TotalAmount));
            var totalNetPay = totalGrossPay - totalTaxDeductions;
            var totalOvertimeHours = paidClaims.Sum(c => c.OvertimeHours ?? 0);
            var totalHoursWorked = paidClaims.Sum(c => c.HoursWorked);

            using (var memoryStream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 40, 40, 40, 40); // Margins
                var writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Add plain text header
                document.Add(new Paragraph($"Claim Report"));
                document.Add(new Paragraph($"Date Range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}"));
                document.Add(new Paragraph(" ")); // Add space

                // Add Processed Claims Section
                document.Add(new Paragraph("Processed Claims (Paid or Approved):"));
                if (paidClaims.Any())
                {
                    foreach (var claims in paidClaims)
                    {
                        document.Add(new Paragraph($"Claim ID: {claims.Id}"));
                        document.Add(new Paragraph($"Lecturer ID: {claims.LecturerID}"));
                        document.Add(new Paragraph($"Hours Worked: {claims.HoursWorked:F2}"));
                        document.Add(new Paragraph($"Overtime Hours: {(claims.OvertimeHours ?? 0):F2}"));
                        document.Add(new Paragraph($"Gross Pay: R{claims.TotalAmount:F2}"));
                        document.Add(new Paragraph($"Net Pay: R{(claims.TotalAmount - CalculateTax(claims.TotalAmount)):F2}"));
                        document.Add(new Paragraph(" ")); // Add space between claims
                    }
                }
                else
                {
                    document.Add(new Paragraph("No processed claims."));
                }

                document.Add(new Paragraph(" ")); // Add space

                // Add Pending or Rejected Claims Section
                document.Add(new Paragraph("Pending or Rejected Claims:"));
                if (pendingOrRejectedClaims.Any())
                {
                    foreach (var claims in pendingOrRejectedClaims)
                    {
                        document.Add(new Paragraph($"Claim ID: {claims.Id}"));
                        document.Add(new Paragraph($"Lecturer ID: {claims.LecturerID}"));
                        document.Add(new Paragraph($"Status: {claims.ClaimStatus}"));
                        document.Add(new Paragraph($"Gross Pay: R{claims.TotalAmount:F2}"));
                        document.Add(new Paragraph($"Date Submitted: {claims.DateSubmitted:yyyy-MM-dd}"));
                        document.Add(new Paragraph(" ")); // Add space between claims
                    }
                }
                else
                {
                    document.Add(new Paragraph("No pending or rejected claims."));
                }

                document.Add(new Paragraph(" ")); // Add space

                // Add Summary Section
                document.Add(new Paragraph("Summary for Processed Claims:"));
                document.Add(new Paragraph($"Total Hours Worked: {totalHoursWorked:F2}"));
                document.Add(new Paragraph($"Total Overtime Hours: {totalOvertimeHours:F2}"));
                document.Add(new Paragraph($"Total Gross Pay: R{totalGrossPay:F2}"));
                document.Add(new Paragraph($"Total Tax Deductions: R{totalTaxDeductions:F2}"));
                document.Add(new Paragraph($"Total Net Pay: R{totalNetPay:F2}"));

                document.Close();
                return memoryStream.ToArray();
            }
        }

        [HttpPost]
        public JsonResult ValidateClaims(string lecturerId, DateTime startDate, DateTime endDate)
        {
            var claims = _context.Claims
                .Where(c => c.UserID == lecturerId && c.DateSubmitted >= startDate && c.DateSubmitted <= endDate)
                .ToList();

            return Json(claims.Any());
        }
    }

    
}
