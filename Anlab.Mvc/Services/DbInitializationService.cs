using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Models.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Text.Encodings.Web;
using Anlab.Core.Models;
using AnlabMvc.Models.Order;

namespace AnlabMvc.Services
{
    public interface IDbInitializationService
    {
        Task Initialize();
        Task RecreateAndInitialize();
    }

    public class DbInitializationService : IDbInitializationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public int TestItemCount = 1000;

        public DbInitializationService(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task RecreateAndInitialize()
        {
            await _context.Database.EnsureDeletedAsync();

            await Initialize();
        }

        public async Task Initialize()
        {

            await _context.Database.EnsureCreatedAsync();

            if (_context.Users.Any()) return; // Do nothing if there is already user data in the system

            // create roles
            await _roleManager.CreateAsync(new IdentityRole(RoleCodes.Admin));
            await _roleManager.CreateAsync(new IdentityRole(RoleCodes.LabUser));
            await _roleManager.CreateAsync(new IdentityRole(RoleCodes.Reports));

            var scottUser = new User
            {
                Email = "srkirkland@ucdavis.edu",
                UserName = "srkirkland@ucdavis.edu",
                Name = "Scott Kirkland"
            };

            var userPrincipal = new ClaimsPrincipal();
            userPrincipal.AddIdentity(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "postit"),
                new Claim(ClaimTypes.Name, "Scott Kirkland")
            }));
            var loginInfo = new ExternalLoginInfo(userPrincipal, "UCDavis", "postit", null);

            await _userManager.CreateAsync(scottUser);
            await _userManager.AddLoginAsync(scottUser, loginInfo);
            await _userManager.AddToRoleAsync(scottUser, RoleCodes.Admin);

            var jasonUser = new User
            {
                Email = "jsylvestre@ucdavis.edu",
                UserName = "jsylvestre@ucdavis.edu",
                Name = "Jason Sylvestre"
            };

            var jasonUserPrincipal = new ClaimsPrincipal();
            userPrincipal.AddIdentity(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "jsylvest"),
                new Claim(ClaimTypes.Name, "Jason Sylvestre")
            }));
            var jasonLoginInfo = new ExternalLoginInfo(jasonUserPrincipal, "UCDavis", "jsylvest", null);

            await _userManager.CreateAsync(jasonUser);
            await _userManager.AddLoginAsync(jasonUser, jasonLoginInfo);
            await _userManager.AddToRoleAsync(jasonUser, RoleCodes.Admin);
            await _userManager.AddToRoleAsync(jasonUser, RoleCodes.LabUser);

            var lauraUser = new User
            {
                Email = "laholstege@ucdavis.edu",
                UserName = "laholstege@ucdavis.edu",
                Name = "Laura Holstege"
            };

            var lauraUserPrincipal = new ClaimsPrincipal();
            userPrincipal.AddIdentity(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "holstege"),
                new Claim(ClaimTypes.Name, "Laura Holstege")
            }));
            var lauraLoginInfo = new ExternalLoginInfo(lauraUserPrincipal, "UCDavis", "holstege", null);

            await _userManager.CreateAsync(lauraUser);
            await _userManager.AddLoginAsync(lauraUser, lauraLoginInfo);
            await _userManager.AddToRoleAsync(lauraUser, RoleCodes.Admin);
            await _userManager.AddToRoleAsync(lauraUser, RoleCodes.LabUser);

            #region Cal's login
            var calUser = new User
            {
                Email = "cydoval@ucdavis.edu",
                UserName = "cydoval@ucdavis.edu",
                Name = "Calvin Doval"
            };

            var calUserPrincipal = new ClaimsPrincipal();
            userPrincipal.AddIdentity(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "cydoval"),
                new Claim(ClaimTypes.Name, "Calvin Doval")
            }));
            var calLoginInfo = new ExternalLoginInfo(calUserPrincipal, "UCDavis", "cydoval", null);

            await _userManager.CreateAsync(calUser);
            await _userManager.AddLoginAsync(calUser, calLoginInfo);
            await _userManager.AddToRoleAsync(calUser, RoleCodes.Admin);


            #endregion Cal's login

            // create a new sample order

            CreateOrders(jasonUser);
            CreateOrders(scottUser);
            CreateOrders(calUser);
            CreateOrders(lauraUser);

            // create the sample methods of analysis
            CreateSop();
            
            // create sample tests

            LoadTestItems();



            await _context.SaveChangesAsync();


            // Seed with orders here, and maybe create users to test with
        }

        private void CreateSop()
        {
            _context.AnalysisMethods.Add(new AnalysisMethod { Id = 505, Title = "Dry Matter Determination for Botanical Materials", Category = "Plant", Content = "lorem ipsum" });
            _context.AnalysisMethods.Add(new AnalysisMethod { Id = 507, Title = "Partial Dry Matter", Category = "Plant", Content = "lorem ipsum" });
            _context.AnalysisMethods.Add(new AnalysisMethod { Id = 200, Title = "Saturated Paste and Saturation Percentage", Category = "Soil", Content = "lorem ipsum" });            
        }

        private void CreateOrders(User user)
        {
            // var xxx = @"{""Quantity"":2,""SampleType"":""Soil"",""AdditionalInfo"":""Sample"",""SelectedTests"":[{""Id"":""PH-S"",""Analysis"":""pH"",""Cost"":20.0,""SetupCost"":45.0,""SubTotal"":40.0,""Total"":85.0},{""Id"":""EC-S"",""Analysis"":""EC"",""Cost"":20.0,""SetupCost"":45.0,""SubTotal"":40.0,""Total"":85.0},{""Id"":""ESP-S"",""Analysis"":""ESP"",""Cost"":0.0,""SetupCost"":0.0,""SubTotal"":0.0,""Total"":0.0},{""Id"":""HCO3-S"",""Analysis"":""HCO3"",""Cost"":20.0,""SetupCost"":45.0,""SubTotal"":40.0,""Total"":85.0},{""Id"":""GRIND"",""Analysis"":""Grind"",""Cost"":9.0,""SetupCost"":45.0,""SubTotal"":18.0,""Total"":63.0},{""Id"":""SP-FOR"",""Analysis"":""Imported Soil"",""Cost"":14.0,""SetupCost"":0.0,""SubTotal"":28.0,""Total"":28.0}],""Total"":346.0,""Payment"":{""ClientType"":""other"",""Account"":null,""IsInternalClient"":false},""AdditionalEmails"":[],""Project"":""2"",""LabComments"":null,""AdjustmentAmount"":0.0,""GrandTotal"":346.0, ""ClientId"":""XYZ"", ""InternalProcessingFee"":30,""ExternalProcessingFee"":45}";
            var yyy = @"[{""Id"":""SORB"",""Code"":null,""InternalCost"":214.0,""ExternalCost"":321.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Sorbitol"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SUCR"",""Code"":null,""InternalCost"":218.0,""ExternalCost"":327.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Sucrose"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""GLUC"",""Code"":null,""InternalCost"":132.0,""ExternalCost"":198.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Glucose"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""FRUC"",""Code"":null,""InternalCost"":131.0,""ExternalCost"":197.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Fructose"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#TNC"",""Code"":null,""InternalCost"":48.0,""ExternalCost"":72.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""TNC"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""FAT-R"",""Code"":null,""InternalCost"":123.0,""ExternalCost"":185.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Fat (Rinse)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TOCOPH"",""Code"":null,""InternalCost"":227.0,""ExternalCost"":341.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""α-Tocopherol *"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CAROTENE"",""Code"":null,""InternalCost"":83.0,""ExternalCost"":125.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""β-Carotene *"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ASCORBIC"",""Code"":null,""InternalCost"":63.0,""ExternalCost"":95.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Ascorbic Acid *"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""PHENOLS"",""Code"":null,""InternalCost"":185.0,""ExternalCost"":278.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Total Phenols *"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TGLUC"",""Code"":null,""InternalCost"":222.0,""ExternalCost"":333.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Total Glucose"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#STARCH"",""Code"":null,""InternalCost"":45.0,""ExternalCost"":68.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Starch"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""PH-M"",""Code"":null,""InternalCost"":186.0,""ExternalCost"":279.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""MANURE and COMPOST TESTS:"",""Analysis"":""pH (water 1:5)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#SOL-HM"",""Code"":null,""InternalCost"":44.0,""ExternalCost"":66.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Soluble Heavy Metals [Cd, Cr, Pb, Ni]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MICRO"",""Code"":null,""InternalCost"":151.0,""ExternalCost"":227.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""OTHER SERVICES REQUESTED:"",""Analysis"":""Acid Digestion (for analysis by ICP-MS)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#WSUIT"",""Code"":null,""InternalCost"":49.0,""ExternalCost"":74.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Water Suitability Group 1 [pH, EC, SAR, Ca, Mg, Na, Cl, B, HCO3, CO3]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#WSUIT-2"",""Code"":null,""InternalCost"":50.0,""ExternalCost"":75.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Water Suitability Group 2 [pH, EC, SAR, Ca, Mg, Na, Cl, B]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#AD-SALTS"",""Code"":null,""InternalCost"":6.0,""ExternalCost"":9.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Acid Digestible Salts [K, Ca, Mg, Na]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#AD-HM"",""Code"":null,""InternalCost"":3.0,""ExternalCost"":5.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Acid Digestible Heavy Metals [Cd, Cr, Pb, Ni]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#CRBBI"",""Code"":null,""InternalCost"":14.0,""ExternalCost"":21.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Bicarbonate & Carbonate [HCO3, CO3]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#IC-PANEL"",""Code"":null,""InternalCost"":21.0,""ExternalCost"":32.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Ion Chromatography Panel [Cl, SO4]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#ANIONS"",""Code"":null,""InternalCost"":7.0,""ExternalCost"":11.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Anion Panel [Cl, SO4-S (soluble S), NO3-N, HCO3]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#SLSLT"",""Code"":null,""InternalCost"":43.0,""ExternalCost"":65.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Soluble Salts [K, Ca, Mg, Na]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#MICRS"",""Code"":null,""InternalCost"":24.0,""ExternalCost"":36.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Soluble Micronutrients [Zn, Mn, Fe, Cu]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""FAT"",""Code"":null,""InternalCost"":122.0,""ExternalCost"":183.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Fat"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""PH-W"",""Code"":null,""InternalCost"":188.0,""ExternalCost"":282.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""pH"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""EC-M"",""Code"":null,""InternalCost"":118.0,""ExternalCost"":177.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""MANURE and COMPOST TESTS:"",""Analysis"":""EC (water 1:5)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ASH"",""Code"":null,""InternalCost"":64.0,""ExternalCost"":96.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Ash"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""DM"",""Code"":null,""InternalCost"":114.0,""ExternalCost"":171.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""DM"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#CELLULOS"",""Code"":null,""InternalCost"":11.0,""ExternalCost"":17.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Cellulose"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CR"",""Code"":null,""InternalCost"":102.0,""ExternalCost"":153.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Cr"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CO"",""Code"":null,""InternalCost"":98.0,""ExternalCost"":147.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Co"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""PB"",""Code"":null,""InternalCost"":181.0,""ExternalCost"":272.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Pb"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MO"",""Code"":null,""InternalCost"":157.0,""ExternalCost"":236.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Mo"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NI"",""Code"":null,""InternalCost"":170.0,""ExternalCost"":255.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Ni"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""V"",""Code"":null,""InternalCost"":233.0,""ExternalCost"":350.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""V"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SE"",""Code"":null,""InternalCost"":201.0,""ExternalCost"":302.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Se"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""AS-PT"",""Code"":null,""InternalCost"":65.0,""ExternalCost"":98.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""As"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CR-OXIDE"",""Code"":null,""InternalCost"":103.0,""ExternalCost"":155.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Cr (oxide)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SI-%"",""Code"":null,""InternalCost"":204.0,""ExternalCost"":306.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Si"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""AL"",""Code"":null,""InternalCost"":56.0,""ExternalCost"":84.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Al"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NO3-P"",""Code"":null,""InternalCost"":173.0,""ExternalCost"":260.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""EXTRACTABLES:"",""Analysis"":""NO3-N"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NH4-P"",""Code"":null,""InternalCost"":167.0,""ExternalCost"":251.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""EXTRACTABLES:"",""Analysis"":""NH4-N"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""K-TOT"",""Code"":null,""InternalCost"":141.0,""ExternalCost"":212.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""EXTRACTABLES:"",""Analysis"":""K"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CL-P-IC"",""Code"":null,""InternalCost"":93.0,""ExternalCost"":140.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""EXTRACTABLES:"",""Analysis"":""Cl"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""PO4-P"",""Code"":null,""InternalCost"":190.0,""ExternalCost"":285.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""EXTRACTABLES:"",""Analysis"":""PO4-P"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SO4-PM"",""Code"":null,""InternalCost"":209.0,""ExternalCost"":314.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""EXTRACTABLES:"",""Analysis"":""SO4-S"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""EC-W"",""Code"":null,""InternalCost"":120.0,""ExternalCost"":180.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""EC"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""DM55"",""Code"":null,""InternalCost"":115.0,""ExternalCost"":173.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Partial DM (dried at 55OC)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#PROT"",""Code"":null,""InternalCost"":38.0,""ExternalCost"":57.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Protein"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ADFRF"",""Code"":null,""InternalCost"":55.0,""ExternalCost"":83.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""ADF"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#ADFAF"",""Code"":null,""InternalCost"":2.0,""ExternalCost"":3.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""ADF (ash free)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#TDNRF"",""Code"":null,""InternalCost"":47.0,""ExternalCost"":71.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""TDN"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#LIG-AF"",""Code"":null,""InternalCost"":22.0,""ExternalCost"":33.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Lignin"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#ADIN"",""Code"":null,""InternalCost"":4.0,""ExternalCost"":6.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""ADIN"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NDFRF"",""Code"":null,""InternalCost"":165.0,""ExternalCost"":248.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""NDF"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#NDFAF"",""Code"":null,""InternalCost"":31.0,""ExternalCost"":47.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""NDF (ash free)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""HEMICELL"",""Code"":null,""InternalCost"":136.0,""ExternalCost"":204.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""FEED TESTS:"",""Analysis"":""Hemicellulose"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TC-W"",""Code"":null,""InternalCost"":220.0,""ExternalCost"":330.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""Total C"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TDS"",""Code"":null,""InternalCost"":221.0,""ExternalCost"":332.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""TDS"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""DOC-W"",""Code"":null,""InternalCost"":117.0,""ExternalCost"":176.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""DOC"",""Description"":null,""Notes"":""DOC-WF for unfiltered samples"",""Public"":true},{""Id"":""NI-W"",""Code"":null,""InternalCost"":171.0,""ExternalCost"":257.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Ni"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CL-WX"",""Code"":null,""InternalCost"":97.0,""ExternalCost"":146.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Cl in mg/L"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CA-WX"",""Code"":null,""InternalCost"":87.0,""ExternalCost"":131.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Ca in mg/L"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MG-WX"",""Code"":null,""InternalCost"":150.0,""ExternalCost"":225.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Mg in mg/L"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NA-WX"",""Code"":null,""InternalCost"":163.0,""ExternalCost"":245.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Na in mg/L"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""P-WT"",""Code"":null,""InternalCost"":197.0,""ExternalCost"":296.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""P"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""K-WT"",""Code"":null,""InternalCost"":142.0,""ExternalCost"":213.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""K"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""S-WT"",""Code"":null,""InternalCost"":219.0,""ExternalCost"":329.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""S"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CA-WT"",""Code"":null,""InternalCost"":86.0,""ExternalCost"":129.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Ca"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MG-WT"",""Code"":null,""InternalCost"":149.0,""ExternalCost"":224.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Mg"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NA-WT"",""Code"":null,""InternalCost"":162.0,""ExternalCost"":243.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Na"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""B-WT"",""Code"":null,""InternalCost"":80.0,""ExternalCost"":120.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""B"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""PB-W"",""Code"":null,""InternalCost"":182.0,""ExternalCost"":273.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Pb"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ZN-WT"",""Code"":null,""InternalCost"":244.0,""ExternalCost"":366.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Zn"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""FE-WT"",""Code"":null,""InternalCost"":130.0,""ExternalCost"":195.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Fe"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CU-WT"",""Code"":null,""InternalCost"":111.0,""ExternalCost"":167.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Cu"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CD-WT"",""Code"":null,""InternalCost"":91.0,""ExternalCost"":137.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Cd"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CR-WT"",""Code"":null,""InternalCost"":105.0,""ExternalCost"":158.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Cr"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""PB-WT"",""Code"":null,""InternalCost"":183.0,""ExternalCost"":275.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Pb"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MO-WT"",""Code"":null,""InternalCost"":158.0,""ExternalCost"":237.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Mo"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NI-WT"",""Code"":null,""InternalCost"":172.0,""ExternalCost"":258.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Ni"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""HG-WT"",""Code"":null,""InternalCost"":137.0,""ExternalCost"":206.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Hg"",""Description"":null,""Notes"":""DO NOT USE"",""Public"":true},{""Id"":""AL-WT"",""Code"":null,""InternalCost"":62.0,""ExternalCost"":93.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Al"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SE-W"",""Code"":null,""InternalCost"":203.0,""ExternalCost"":305.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Se"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""AS-WT"",""Code"":null,""InternalCost"":67.0,""ExternalCost"":101.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""As"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SO4-W-IC"",""Code"":null,""InternalCost"":213.0,""ExternalCost"":320.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""SO4 (Ion Chromatography)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MN-WT"",""Code"":null,""InternalCost"":156.0,""ExternalCost"":234.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ACID DIGESTIBLE MINERALS:"",""Analysis"":""Mn"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CR-W"",""Code"":null,""InternalCost"":104.0,""ExternalCost"":156.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Cr"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CD-W"",""Code"":null,""InternalCost"":90.0,""ExternalCost"":135.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Cd"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""V-W\t "",""Code"":null,""InternalCost"":235.0,""ExternalCost"":353.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""V"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TN-W"",""Code"":null,""InternalCost"":226.0,""ExternalCost"":339.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""Total N"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TKN-W"",""Code"":null,""InternalCost"":225.0,""ExternalCost"":338.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""TKN"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#SAR-W"",""Code"":null,""InternalCost"":42.0,""ExternalCost"":63.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""SAR"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#ESP-W"",""Code"":null,""InternalCost"":15.0,""ExternalCost"":23.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""ESP"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#HARD"",""Code"":null,""InternalCost"":20.0,""ExternalCost"":30.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""Hardness"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CL-W"",""Code"":null,""InternalCost"":95.0,""ExternalCost"":143.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""Cl"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""HCO3-W"",""Code"":null,""InternalCost"":135.0,""ExternalCost"":203.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""HCO3"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CO3-W"",""Code"":null,""InternalCost"":100.0,""ExternalCost"":150.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""CO3"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""P-W"",""Code"":null,""InternalCost"":195.0,""ExternalCost"":293.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""PO4-P (soluble P)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CD"",""Code"":null,""InternalCost"":88.0,""ExternalCost"":132.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Cd"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TS"",""Code"":null,""InternalCost"":230.0,""ExternalCost"":345.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""TS"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TSS"",""Code"":null,""InternalCost"":231.0,""ExternalCost"":347.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""TSS"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""VSS"",""Code"":null,""InternalCost"":234.0,""ExternalCost"":351.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""VSS"",""Description"":null,""Notes"":""requires TSS"",""Public"":true},{""Id"":""ALK"",""Code"":null,""InternalCost"":57.0,""ExternalCost"":86.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""Alkalinity"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TURBID"",""Code"":null,""InternalCost"":232.0,""ExternalCost"":348.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""Turbidity"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""K-SOLW"",""Code"":null,""InternalCost"":139.0,""ExternalCost"":209.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""K"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SO4-W"",""Code"":null,""InternalCost"":212.0,""ExternalCost"":318.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""SO4-S (soluble S)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CA-W"",""Code"":null,""InternalCost"":85.0,""ExternalCost"":128.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Ca"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MG-W"",""Code"":null,""InternalCost"":148.0,""ExternalCost"":222.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Mg"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NA-W"",""Code"":null,""InternalCost"":161.0,""ExternalCost"":242.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Na"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""B-W\t "",""Code"":null,""InternalCost"":79.0,""ExternalCost"":119.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""B"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ZN-W"",""Code"":null,""InternalCost"":243.0,""ExternalCost"":365.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Zn"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MN-W"",""Code"":null,""InternalCost"":155.0,""ExternalCost"":233.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Mn"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""FE-W"",""Code"":null,""InternalCost"":129.0,""ExternalCost"":194.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Fe"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CU-W"",""Code"":null,""InternalCost"":110.0,""ExternalCost"":165.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Cu"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""AL-W"",""Code"":null,""InternalCost"":61.0,""ExternalCost"":92.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Al"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SI-W"",""Code"":null,""InternalCost"":207.0,""ExternalCost"":311.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""SOLUBLE MINERALS:"",""Analysis"":""Si"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TOC-W"",""Code"":null,""InternalCost"":229.0,""ExternalCost"":344.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""???"",""Analysis"":""TOC"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""BA-PMF"",""Code"":null,""InternalCost"":73.0,""ExternalCost"":110.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Ba"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TKN-P"",""Code"":null,""InternalCost"":223.0,""ExternalCost"":335.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""TOTALS:"",""Analysis"":""TKN"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""FE-PM"",""Code"":null,""InternalCost"":126.0,""ExternalCost"":189.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Fe"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""P-OS"",""Code"":null,""InternalCost"":191.0,""ExternalCost"":287.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""Olsen-P"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SO4-S"",""Code"":null,""InternalCost"":210.0,""ExternalCost"":315.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""SO4-S"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#XK-X"",""Code"":null,""InternalCost"":52.0,""ExternalCost"":78.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""X-K"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""X-NA"",""Code"":null,""InternalCost"":239.0,""ExternalCost"":359.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""X-Na"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""X-CA"",""Code"":null,""InternalCost"":237.0,""ExternalCost"":356.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""X-Ca"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""X-MG"",""Code"":null,""InternalCost"":238.0,""ExternalCost"":357.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""X-Mg"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ZN-S"",""Code"":null,""InternalCost"":242.0,""ExternalCost"":363.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""Zn (DTPA)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MN-S"",""Code"":null,""InternalCost"":154.0,""ExternalCost"":231.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""Mn (DTPA)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""FE-S"",""Code"":null,""InternalCost"":128.0,""ExternalCost"":192.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""Fe (DTPA)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CU-S"",""Code"":null,""InternalCost"":109.0,""ExternalCost"":164.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""Cu (DTPA)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""AL-KCL"",""Code"":null,""InternalCost"":58.0,""ExternalCost"":87.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""Al (KCl Extraction)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SP"",""Code"":null,""InternalCost"":215.0,""ExternalCost"":323.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""SP"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""BRAY-P"",""Code"":null,""InternalCost"":77.0,""ExternalCost"":116.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""Bray-P"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""PH-S"",""Code"":null,""InternalCost"":187.0,""ExternalCost"":281.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""pH"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SAR-S"",""Code"":null,""InternalCost"":199.0,""ExternalCost"":299.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""SAR"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ESP-S"",""Code"":null,""InternalCost"":121.0,""ExternalCost"":182.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""ESP"",""Description"":null,""Notes"":""We use our special powers to figure this out."",""Public"":true},{""Id"":""CA-S"",""Code"":null,""InternalCost"":84.0,""ExternalCost"":126.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""Ca"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MG-S"",""Code"":null,""InternalCost"":147.0,""ExternalCost"":221.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""Mg"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NA-S"",""Code"":null,""InternalCost"":160.0,""ExternalCost"":240.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""Na"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CL-S"",""Code"":null,""InternalCost"":94.0,""ExternalCost"":141.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""Cl"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""B-S"",""Code"":null,""InternalCost"":78.0,""ExternalCost"":117.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""B"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""K-SOLS"",""Code"":null,""InternalCost"":138.0,""ExternalCost"":207.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""K"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NO3-SP"",""Code"":null,""InternalCost"":175.0,""ExternalCost"":263.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""NO3-N"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""HCO3-S"",""Code"":null,""InternalCost"":134.0,""ExternalCost"":201.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""HCO3"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CO3-S"",""Code"":null,""InternalCost"":99.0,""ExternalCost"":149.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""CO3"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SO4-SP"",""Code"":null,""InternalCost"":211.0,""ExternalCost"":317.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""SO4-S"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""EC-S"",""Code"":null,""InternalCost"":119.0,""ExternalCost"":179.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""SAT PASTE EXT:"",""Analysis"":""EC"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NH4F-S"",""Code"":null,""InternalCost"":166.0,""ExternalCost"":249.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""NH4-N"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NO3-S"",""Code"":null,""InternalCost"":174.0,""ExternalCost"":261.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""NO3-N"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TKN-S"",""Code"":null,""InternalCost"":224.0,""ExternalCost"":336.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""TKN"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""PROC"",""Code"":null,""InternalCost"":30.0,""ExternalCost"":45.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil|Plant"",""Categories"":[""Soil"",""Plant""],""Group"":""Special"",""Analysis"":""Processing Fee"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""-BCL-P-IC"",""Code"":null,""InternalCost"":74.0,""ExternalCost"":111.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Just to get it to pass"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""-BNA-PMF"",""Code"":null,""InternalCost"":75.0,""ExternalCost"":113.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Just to get it to pass"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""-DCL-P-IC"",""Code"":null,""InternalCost"":113.0,""ExternalCost"":170.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Just to get it to pass"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""-DNA-PMF"",""Code"":null,""InternalCost"":116.0,""ExternalCost"":174.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Just to get it to pass"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""-LCL-P-IC"",""Code"":null,""InternalCost"":143.0,""ExternalCost"":215.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Just to get it to pass"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""-LNA-PMF"",""Code"":null,""InternalCost"":144.0,""ExternalCost"":216.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Just to get it to pass"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""-PCL-P-IC"",""Code"":null,""InternalCost"":184.0,""ExternalCost"":276.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Just to get it to pass"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""-PNA-PMF"",""Code"":null,""InternalCost"":189.0,""ExternalCost"":284.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Just to get it to pass"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""-SCL-P-IC"",""Code"":null,""InternalCost"":200.0,""ExternalCost"":300.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Just to get it to pass"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""-SNA-PMF"",""Code"":null,""InternalCost"":208.0,""ExternalCost"":312.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Just to get it to pass"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""D"",""Code"":null,""InternalCost"":112.0,""ExternalCost"":168.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Just to get it to pass"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""M"",""Code"":null,""InternalCost"":145.0,""ExternalCost"":218.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Just to get it to pass"",""Description"":null,""Notes"":null,""Public"":false},{""Id"":""GRIND"",""Code"":null,""InternalCost"":133.0,""ExternalCost"":200.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil|Plant"",""Categories"":[""Soil"",""Plant""],""Group"":""Special"",""Analysis"":""Grind"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SP-FOR"",""Code"":null,""InternalCost"":216.0,""ExternalCost"":324.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""Special"",""Analysis"":""Imported Soil"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#SALIN"",""Code"":null,""InternalCost"":40.0,""ExternalCost"":60.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Soil Salinity Group 1 [SP, pH, EC, Ca, Mg, Na, Cl, B, HCO3, CO3]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#SALIN-2"",""Code"":null,""InternalCost"":41.0,""ExternalCost"":62.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Soil Salinity Group 2 [SP, pH, EC, Ca, Mg, Na, Cl, B]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#FERT"",""Code"":null,""InternalCost"":18.0,""ExternalCost"":27.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Soil Fertility Group 1 [NO3-N, Olsen-P, X-K]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#FERT2"",""Code"":null,""InternalCost"":19.0,""ExternalCost"":29.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Soil Fertility Group 2 [NO3-N, Olsen-P, X-K, X-Na, X-Ca, X-Mg, OM (LOI), pH, CEC (Estimated)]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#NC-S"",""Code"":null,""InternalCost"":30.0,""ExternalCost"":45.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Total Nitrogen & Carbon [N, C]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#NAF-S"",""Code"":null,""InternalCost"":26.0,""ExternalCost"":39.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Nitrate & Ammonium [NO3-N, NH4-N]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#XCAT"",""Code"":null,""InternalCost"":51.0,""ExternalCost"":77.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Exchangeable Cations [X-K, X-Na, X-Ca, X-Mg]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#MICRE"",""Code"":null,""InternalCost"":23.0,""ExternalCost"":35.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Extractable Micronutrients [DTPA: Zn, Mn, Fe, Cu]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#AD-MICR"",""Code"":null,""InternalCost"":5.0,""ExternalCost"":8.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil|Water"",""Categories"":[""Soil"",""Water""],""Group"":""DISCOUNTED GROUPS:"",""Analysis"":""Acid Digestible Micronutrients [Zn, Mn, Fe, Cu]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""TOC-S"",""Code"":null,""InternalCost"":228.0,""ExternalCost"":342.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""TOC"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""C-S"",""Code"":null,""InternalCost"":106.0,""ExternalCost"":159.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""C"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""N-CE-S"",""Code"":null,""InternalCost"":164.0,""ExternalCost"":246.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""FERTILITY:"",""Analysis"":""N"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CEC"",""Code"":null,""InternalCost"":92.0,""ExternalCost"":138.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""PHYSIO CHEM:"",""Analysis"":""CEC"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#CEC-EST"",""Code"":null,""InternalCost"":10.0,""ExternalCost"":15.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""PHYSIO CHEM:"",""Analysis"":""CEC (Estimated)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""OM-LOI"",""Code"":null,""InternalCost"":180.0,""ExternalCost"":270.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""PHYSIO CHEM:"",""Analysis"":""OM (LOI)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#CORG-LOI"",""Code"":null,""InternalCost"":13.0,""ExternalCost"":20.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""PHYSIO CHEM:"",""Analysis"":""Org.C (LOI)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#NUTRA2"",""Code"":null,""InternalCost"":33.0,""ExternalCost"":50.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Nutrient Panel A [N, P, K]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#NUTRB"",""Code"":null,""InternalCost"":34.0,""ExternalCost"":51.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Nutrient Panel B [S, B, Ca, Mg]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#NUTRC"",""Code"":null,""InternalCost"":35.0,""ExternalCost"":53.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Nutrient Panel C [Zn, Mn, Fe, Cu] "",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#PLANT-D3"",""Code"":null,""InternalCost"":36.0,""ExternalCost"":54.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Nutrient Panel D [Panels A, B & C tests"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#PLANT-E"",""Code"":null,""InternalCost"":37.0,""ExternalCost"":56.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Nutrient Panel E [NO3-N, PO4-P, K, Panels B & C tests] "",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#NA-PMF"",""Code"":null,""InternalCost"":28.0,""ExternalCost"":42.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Add Na to a Nutrient Panel"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#NC-P"",""Code"":null,""InternalCost"":29.0,""ExternalCost"":44.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Total Nitrogen & Carbon [N, C]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#NA-P"",""Code"":null,""InternalCost"":27.0,""ExternalCost"":41.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Nitrate & Ammonium [NO3-N, NH4-N]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#XNPK"",""Code"":null,""InternalCost"":53.0,""ExternalCost"":80.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Extractable N-P-K Group 1 [NO3-N, NH4-N, PO4-P, K]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#XNPKF-2"",""Code"":null,""InternalCost"":54.0,""ExternalCost"":81.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Extractable N-P-K Group 2 [NO3-N, PO4-P, K]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#CARBS"",""Code"":null,""InternalCost"":8.0,""ExternalCost"":12.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Soluble Carbohydrates Group 1 [Fructose, Glucose, Sucrose]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#CARBS2"",""Code"":null,""InternalCost"":9.0,""ExternalCost"":14.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Soluble Carbohydrates Group 2 [Fructose, Glucose, Sucrose, Sorbitol]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#TCARB"",""Code"":null,""InternalCost"":46.0,""ExternalCost"":69.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Carbohydrate Panel [TNC, Starch, Fructose, Glucose, Sucrose, Total Glucose]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#FEED1"",""Code"":null,""InternalCost"":16.0,""ExternalCost"":24.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Feed Group 1 [DM, Protein, ADF, TDN]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#FEED2"",""Code"":null,""InternalCost"":17.0,""ExternalCost"":26.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""DISCOUNTED GROUPS: "",""Analysis"":""Feed Group 2 [DM, Protein, ADF, TDN, Ash, Fat]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""C-P"",""Code"":null,""InternalCost"":101.0,""ExternalCost"":152.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""TOTALS:"",""Analysis"":""C"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""N-P"",""Code"":null,""InternalCost"":178.0,""ExternalCost"":267.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""TOTALS:"",""Analysis"":""N"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CL-W-IC"",""Code"":null,""InternalCost"":96.0,""ExternalCost"":144.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""Cl (Ion Chromatography)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""P-TOT"",""Code"":null,""InternalCost"":194.0,""ExternalCost"":291.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""P"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""K-TMW"",""Code"":null,""InternalCost"":140.0,""ExternalCost"":210.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""K"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""S-TOT"",""Code"":null,""InternalCost"":217.0,""ExternalCost"":326.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""S"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""B-PMF"",""Code"":null,""InternalCost"":76.0,""ExternalCost"":114.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""B"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CA-PMF"",""Code"":null,""InternalCost"":82.0,""ExternalCost"":123.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Ca"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MG-PMF"",""Code"":null,""InternalCost"":146.0,""ExternalCost"":219.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Mg"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NA-PMF"",""Code"":null,""InternalCost"":159.0,""ExternalCost"":239.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Na"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ZN-PM"",""Code"":null,""InternalCost"":241.0,""ExternalCost"":362.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Zn"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MN-PM"",""Code"":null,""InternalCost"":153.0,""ExternalCost"":230.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Mn"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""WRAP"",""Code"":null,""InternalCost"":236.0,""ExternalCost"":354.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil|Plant"",""Categories"":[""Soil"",""Plant""],""Group"":""OTHER SERVICES REQUESTED:"",""Analysis"":""Sample Encapsulation (for N &/or C isotope testing)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CU-PM"",""Code"":null,""InternalCost"":108.0,""ExternalCost"":162.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Plant"",""Categories"":[""Plant""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Cu"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#NA-E"",""Code"":null,""InternalCost"":25.0,""ExternalCost"":38.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil|Water"",""Categories"":[""Soil"",""Water""],""Group"":""TESTS ON CLIENT-PROVIDED EXTRACTS:"",""Analysis"":""Nitrate & Ammonium [NO3-N, NH4-N]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NO3-W"",""Code"":null,""InternalCost"":177.0,""ExternalCost"":266.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil|Water"",""Categories"":[""Soil"",""Water""],""Group"":""TESTS ON CLIENT-PROVIDED EXTRACTS:"",""Analysis"":""NO3-N"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CACO3"",""Code"":null,""InternalCost"":81.0,""ExternalCost"":122.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""PHYSIO CHEM:"",""Analysis"":""CaCO3"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ATMP3"",""Code"":null,""InternalCost"":72.0,""ExternalCost"":108.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""PHYSIO CHEM:"",""Analysis"":""Moisture Retention: 0.33 atm"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ATM1"",""Code"":null,""InternalCost"":68.0,""ExternalCost"":102.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""PHYSIO CHEM:"",""Analysis"":""Moisture Retention: 1 atm"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ATM5"",""Code"":null,""InternalCost"":71.0,""ExternalCost"":107.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""PHYSIO CHEM:"",""Analysis"":""Moisture Retention: 5 atm"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ATM10"",""Code"":null,""InternalCost"":69.0,""ExternalCost"":104.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""PHYSIO CHEM:"",""Analysis"":""Moisture Retention: 10 atm"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ATM15"",""Code"":null,""InternalCost"":70.0,""ExternalCost"":105.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""PHYSIO CHEM:"",""Analysis"":""Moisture Retention: 15 atm\t"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#PSA"",""Code"":null,""InternalCost"":39.0,""ExternalCost"":59.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""PHYSIO CHEM:"",""Analysis"":""Particle Size [Sand/Silt/Clay]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SANDVF"",""Code"":null,""InternalCost"":198.0,""ExternalCost"":297.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""PHYSIO CHEM:"",""Analysis"":""Very Fine Sand"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""P-ST"",""Code"":null,""InternalCost"":193.0,""ExternalCost"":290.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""P "",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""ZN_TOT"",""Code"":null,""InternalCost"":240.0,""ExternalCost"":360.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Zn"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""MN_TOT"",""Code"":null,""InternalCost"":152.0,""ExternalCost"":228.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Mn"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""FE_TOT"",""Code"":null,""InternalCost"":124.0,""ExternalCost"":186.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Fe"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CU_TOT"",""Code"":null,""InternalCost"":107.0,""ExternalCost"":161.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Cu"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""CD_TOT"",""Code"":null,""InternalCost"":89.0,""ExternalCost"":134.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Cd"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SE-ST"",""Code"":null,""InternalCost"":202.0,""ExternalCost"":303.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""Se"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""AS-ST"",""Code"":null,""InternalCost"":66.0,""ExternalCost"":99.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ACID DIGESTIBLES:"",""Analysis"":""As"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""OM"",""Code"":null,""InternalCost"":179.0,""ExternalCost"":269.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""OM (Walkley-Black)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#CORG"",""Code"":null,""InternalCost"":12.0,""ExternalCost"":18.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""Org.C (W-B)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""AL-OX"",""Code"":null,""InternalCost"":59.0,""ExternalCost"":89.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""Al"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""FE-OX"",""Code"":null,""InternalCost"":125.0,""ExternalCost"":188.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""Fe"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SI-OX"",""Code"":null,""InternalCost"":205.0,""ExternalCost"":308.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""Si"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""AL-PY"",""Code"":null,""InternalCost"":60.0,""ExternalCost"":90.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""Al"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""FE-PY"",""Code"":null,""InternalCost"":127.0,""ExternalCost"":191.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""Fe "",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""SI-PY"",""Code"":null,""InternalCost"":206.0,""ExternalCost"":309.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""Si"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NO3S-WET"",""Code"":null,""InternalCost"":176.0,""ExternalCost"":264.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""NO3-N (undried soil)"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NH4S-WET"",""Code"":null,""InternalCost"":168.0,""ExternalCost"":252.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""NH4-N (undried soil) "",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""#NN-WET\t"",""Code"":null,""InternalCost"":32.0,""ExternalCost"":48.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil"",""Categories"":[""Soil""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""Nitrate & Ammonium (undried soil) [NO3-N, NH4-N]"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""NH4-W"",""Code"":null,""InternalCost"":169.0,""ExternalCost"":254.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Soil|Water"",""Categories"":[""Soil"",""Water""],""Group"":""TESTS ON CLIENT-PROVIDED EXTRACTS:"",""Analysis"":""NH4-N"",""Description"":null,""Notes"":null,""Public"":true},{""Id"":""P-W-ICP"",""Code"":null,""InternalCost"":196.0,""ExternalCost"":294.0,""InternalSetupCost"":30.0,""ExternalSetupCost"":45.0,""Category"":""Water"",""Categories"":[""Water""],""Group"":""ALTERNATE METHODS:"",""Analysis"":""Cl (Ion Chromatography)"",""Description"":null,""Notes"":null,""Public"":true}]";

            var cc1 = @"{""Quantity"":2,""SampleType"":""Soil"",""AdditionalInfo"":null,""SelectedTests"":[{""Id"":""P-OS"",""Analysis"":""Olsen-P"",""Cost"":287.0,""SetupCost"":45.0,""SubTotal"":574.0,""Total"":619.0},{""Id"":""SO4-S"",""Analysis"":""SO4-S"",""Cost"":315.0,""SetupCost"":45.0,""SubTotal"":630.0,""Total"":675.0}],""Total"":1339.0,""Payment"":{""ClientType"":""other"",""Account"":null,""IsInternalClient"":false},""AdditionalEmails"":[],""Project"":""Credit Card"",""LabComments"":null,""AdjustmentAmount"":0.0,""GrandTotal"":1339.0,""ClientInfo"":{""ClientId"":""7654321"",""Email"":""sub@fake.com"",""CopyEmail"":""othercopy@fake.com"",""Employer"":null,""Name"":""Name, Fake"",""PhoneNumber"":null},""InternalProcessingFee"":30.0,""ExternalProcessingFee"":45.0}";
            var ucdAccount = @"{""Quantity"":1,""SampleType"":""Soil"",""AdditionalInfo"":null,""SelectedTests"":[{""Id"":""P-OS"",""Analysis"":""Olsen-P"",""Cost"":191.0,""SetupCost"":30.0,""SubTotal"":191.0,""Total"":221.0},{""Id"":""SO4-S"",""Analysis"":""SO4-S"",""Cost"":210.0,""SetupCost"":30.0,""SubTotal"":210.0,""Total"":240.0}],""Total"":491.0,""Payment"":{""ClientType"":""uc"",""Account"":""3-BG13DOE"",""IsInternalClient"":true},""AdditionalEmails"":[],""Project"":""Uc Davis Account"",""LabComments"":null,""AdjustmentAmount"":0.0,""GrandTotal"":491.0,""ClientInfo"":{""ClientId"":""7654321"",""Email"":""sub@fake.com"",""CopyEmail"":""othercopy@fake.com"",""Employer"":null,""Name"":""Name, Fake"",""PhoneNumber"":null},""InternalProcessingFee"":30.0,""ExternalProcessingFee"":45.0}";
            var nonUcAccount = @"{""Quantity"":2,""SampleType"":""Soil"",""AdditionalInfo"":null,""SelectedTests"":[{""Id"":""P-OS"",""Analysis"":""Olsen-P"",""Cost"":191.0,""SetupCost"":30.0,""SubTotal"":382.0,""Total"":412.0},{""Id"":""SO4-S"",""Analysis"":""SO4-S"",""Cost"":210.0,""SetupCost"":30.0,""SubTotal"":420.0,""Total"":450.0}],""Total"":892.0,""Payment"":{""ClientType"":""uc"",""Account"":""X-1234567"",""IsInternalClient"":true},""AdditionalEmails"":[],""Project"":""Non Uc Account"",""LabComments"":null,""AdjustmentAmount"":0.0,""GrandTotal"":892.0,""ClientInfo"":{""ClientId"":""7654321"",""Email"":""sub@fake.com"",""CopyEmail"":""othercopy@fake.com"",""Employer"":null,""Name"":""Name, Fake"",""PhoneNumber"":null},""InternalProcessingFee"":30.0,""ExternalProcessingFee"":45.0}";

            #region Credit Card Examples
            var order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "CreditCard",
                Status = OrderStatusCodes.Created,
                PaymentType = PaymentTypeCodes.CreditCard,
                JsonDetails = cc1,
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "CreditCard",
                Status = OrderStatusCodes.Confirmed,
                PaymentType = PaymentTypeCodes.CreditCard,
                JsonDetails = cc1,
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "CreditCard",
                Status = OrderStatusCodes.Received,
                PaymentType = PaymentTypeCodes.CreditCard,
                RequestNum = "17P138",
                JsonDetails = cc1,
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "CreditCard",
                Status = OrderStatusCodes.Finalized,
                PaymentType = PaymentTypeCodes.CreditCard,
                RequestNum = "17P138",
                JsonDetails = cc1,
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);

            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "CreditCard",
                Status = OrderStatusCodes.Complete,
                PaymentType = PaymentTypeCodes.CreditCard,
                Paid = true,
                RequestNum = "17P138",
                JsonDetails = cc1,
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
            #endregion

            #region UC Davis Account Examples
            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "UC Davis Account",
                Status = OrderStatusCodes.Created,
                PaymentType = PaymentTypeCodes.UcDavisAccount,
                JsonDetails = ucdAccount,
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "UC Davis Account",
                Status = OrderStatusCodes.Confirmed,
                PaymentType = PaymentTypeCodes.UcDavisAccount,
                JsonDetails = ucdAccount,
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "UC Davis Account",
                Status = OrderStatusCodes.Received,
                PaymentType = PaymentTypeCodes.UcDavisAccount,
                JsonDetails = ucdAccount,
                RequestNum = "17P138",
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "UC Davis Account",
                Status = OrderStatusCodes.Finalized,
                PaymentType = PaymentTypeCodes.UcDavisAccount,
                JsonDetails = ucdAccount,
                RequestNum = "17P138",
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);

            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "UC Davis Account",
                Status = OrderStatusCodes.Complete,
                PaymentType = PaymentTypeCodes.UcDavisAccount,
                JsonDetails = ucdAccount,
                Paid = true,
                RequestNum = "17P138",
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
            #endregion

            #region NON UC Davis Account Examples
            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Non UC Davis Account",
                Status = OrderStatusCodes.Created,
                PaymentType = PaymentTypeCodes.UcOtherAccount,
                JsonDetails = nonUcAccount,
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Non UC Davis Account",
                Status = OrderStatusCodes.Confirmed,
                PaymentType = PaymentTypeCodes.UcOtherAccount,
                JsonDetails = nonUcAccount,
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Non UC Davis Account",
                Status = OrderStatusCodes.Received,
                PaymentType = PaymentTypeCodes.UcOtherAccount,
                JsonDetails = nonUcAccount,
                RequestNum = "17P138",
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Non UC Davis Account",
                Status = OrderStatusCodes.Finalized,
                PaymentType = PaymentTypeCodes.UcOtherAccount,
                JsonDetails = nonUcAccount,
                RequestNum = "17P138",
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);

            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Non UC Davis Account",
                Status = OrderStatusCodes.Complete,
                PaymentType = PaymentTypeCodes.UcOtherAccount,
                JsonDetails = nonUcAccount,
                Paid = true,
                RequestNum = "17P138",
                SavedTestDetails = yyy,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
            #endregion

        }

        private void LoadTestItems()
        {
            CreateTestItem("PROC", "Processing Fee", string.Format("{0}|{1}", TestCategories.Soil, TestCategories.Plant), "Special", null, false);

            //Meh
            //CreateTestItem("()", "Just to get it to pass", TestCategories.Soil, "Special", null, false); //We now filter this out in the query
            CreateTestItem("-BCL-P-IC", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-BNA-PMF", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-DCL-P-IC", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-DNA-PMF", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-LCL-P-IC", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-LNA-PMF", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-PCL-P-IC", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-PNA-PMF", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-SCL-P-IC", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-SNA-PMF", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("D", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("M", "Just to get it to pass", TestCategories.Soil, "Special", null, false);

            var faker =
                @"-LZN-S,-PZN_TOT,-BZN_TOT,-SCD_TOT,-SZN_TOT,-LZN_TOT,-PZN-S,-DZN-S,-SZN-S,-LCD_TOT,-DZN_TOT,-BZN-S,-DCD_TOT,-BCD_TOT,-PCD_TOT,G,-DXK,-SXK,-SP-OS,-PXK,-PP-OS,-LXK-X,-BXK,-BP-OS,G-XK-X,-LP-OS,-SXK-X,-LXK,-DXK-X,XK-X,-DP-OS,XK,-BXK-X,-PXK-X,-DN-P,-LFAT,-DTDNRF,-DFAT,-PN-P,-PADFRF,-DADFRF,-LTDNRF,-PTDNRF,-LPROT,-LADFRF,-BPROT,-PFAT,-SN-P,-LASH,-DPROT,-SADFRF,-SASH,TDNRF,-STDNRF,-SFAT,-DASH,-BFAT,-SPROT,PROT,-PPROT,-LN-P,G-FEED2,-PASH,-BN-P".Split(',');
            foreach (var s in faker)
            {
                CreateTestItem(s, s, TestCategories.Soil, "Special", null, false);
            }
            //Soil
            CreateTestItem("GRIND", "Grind", string.Format("{0}|{1}", TestCategories.Soil, TestCategories.Plant), "Special");
            CreateTestItem("SP-FOR", "Imported Soil", TestCategories.Soil, "Special");

            CreateTestItem("#SALIN", "Soil Salinity Group 1 [SP, pH, EC, Ca, Mg, Na, Cl, B, HCO3, CO3]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#SALIN-2", "Soil Salinity Group 2 [SP, pH, EC, Ca, Mg, Na, Cl, B]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#FERT", "Soil Fertility Group 1 [NO3-N, Olsen-P, X-K]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#FERT2", "Soil Fertility Group 2 [NO3-N, Olsen-P, X-K, X-Na, X-Ca, X-Mg, OM (LOI), pH, CEC (Estimated)]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#NC-S", "Total Nitrogen & Carbon [N, C]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#NAF-S", "Nitrate & Ammonium [NO3-N, NH4-N]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#XCAT", "Exchangeable Cations [X-K, X-Na, X-Ca, X-Mg]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#MICRE", "Extractable Micronutrients [DTPA: Zn, Mn, Fe, Cu]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#AD-MICR", "Acid Digestible Micronutrients [Zn, Mn, Fe, Cu]", string.Format("{0}|{1}", TestCategories.Soil, TestCategories.Water), "DISCOUNTED GROUPS:");

            CreateTestItem("TOC-S", "TOC", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("C-S", "C", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("N-CE-S", "N", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("TKN-S", "TKN", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("NO3-S", "NO3-N", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("NH4F-S", "NH4-N", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("BRAY-P", "Bray-P", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("P-OS", "Olsen-P", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("SO4-S", "SO4-S", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("#XK-X", "X-K", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("X-NA", "X-Na", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("X-CA", "X-Ca", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("X-MG", "X-Mg", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("ZN-S", "Zn (DTPA)", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("MN-S", "Mn (DTPA)", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("FE-S", "Fe (DTPA)", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("CU-S", "Cu (DTPA)", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("AL-KCL", "Al (KCl Extraction)", TestCategories.Soil, "FERTILITY:");

            CreateTestItem("SP", "SP", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("PH-S", "pH", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("EC-S", "EC", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("SAR-S", "SAR", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("ESP-S", "ESP", TestCategories.Soil, "SAT PASTE EXT:", "We use our special powers to figure this out.");
            CreateTestItem("CA-S", "Ca", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("MG-S", "Mg", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("NA-S", "Na", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("CL-S", "Cl", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("B-S", "B", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("K-SOLS", "K", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("NO3-SP", "NO3-N", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("HCO3-S", "HCO3", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("CO3-S", "CO3", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("SO4-SP", "SO4-S", TestCategories.Soil, "SAT PASTE EXT:");


            CreateTestItem("CEC", "CEC", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("#CEC-EST", "CEC (Estimated)", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("OM-LOI", "OM (LOI)", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("#CORG-LOI", "Org.C (LOI)", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("CACO3", "CaCO3", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("ATMP3", "Moisture Retention: 0.33 atm", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("ATM1", "Moisture Retention: 1 atm", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("ATM5", "Moisture Retention: 5 atm", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("ATM10", "Moisture Retention: 10 atm", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("ATM15", "Moisture Retention: 15 atm	", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("#PSA", "Particle Size [Sand/Silt/Clay]", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("SANDVF", "Very Fine Sand", TestCategories.Soil, "PHYSIO CHEM:");

            CreateTestItem("P-ST", "P ", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("ZN_TOT", "Zn", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("MN_TOT", "Mn", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("FE_TOT", "Fe", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("CU_TOT", "Cu", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("CD_TOT", "Cd", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("SE-ST", "Se", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("AS-ST", "As", TestCategories.Soil, "ACID DIGESTIBLES:");

            CreateTestItem("OM", "OM (Walkley-Black)", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("#CORG", "Org.C (W-B)", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("AL-OX", "Al", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("FE-OX", "Fe", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("SI-OX", "Si", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("AL-PY", "Al", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("FE-PY", "Fe ", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("SI-PY", "Si", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("NO3S-WET", "NO3-N (undried soil)", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("NH4S-WET", "NH4-N (undried soil) ", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("#NN-WET	", "Nitrate & Ammonium (undried soil) [NO3-N, NH4-N]", TestCategories.Soil, "ALTERNATE METHODS:");

            CreateTestItem("NO3-W", "NO3-N", string.Format("{0}|{1}", TestCategories.Soil, TestCategories.Water), "TESTS ON CLIENT-PROVIDED EXTRACTS:");
            CreateTestItem("NH4-W", "NH4-N", string.Format("{0}|{1}", TestCategories.Soil, TestCategories.Water), "TESTS ON CLIENT-PROVIDED EXTRACTS:");
            CreateTestItem("#NA-E", "Nitrate & Ammonium [NO3-N, NH4-N]", string.Format("{0}|{1}", TestCategories.Soil, TestCategories.Water), "TESTS ON CLIENT-PROVIDED EXTRACTS:");

            CreateTestItem("WRAP", "Sample Encapsulation (for N &/or C isotope testing)", string.Format("{0}|{1}", TestCategories.Soil, TestCategories.Plant), "OTHER SERVICES REQUESTED:");

            //Plant

            CreateTestItem("#NUTRA2", "Nutrient Panel A [N, P, K]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#NUTRB", "Nutrient Panel B [S, B, Ca, Mg]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#NUTRC", "Nutrient Panel C [Zn, Mn, Fe, Cu] ", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#PLANT-D3", "Nutrient Panel D [Panels A, B & C tests", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#PLANT-E", "Nutrient Panel E [NO3-N, PO4-P, K, Panels B & C tests] ", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#NA-PMF", "Add Na to a Nutrient Panel", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#NC-P", "Total Nitrogen & Carbon [N, C]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#NA-P", "Nitrate & Ammonium [NO3-N, NH4-N]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#XNPK", "Extractable N-P-K Group 1 [NO3-N, NH4-N, PO4-P, K]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#XNPKF-2", "Extractable N-P-K Group 2 [NO3-N, PO4-P, K]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#CARBS", "Soluble Carbohydrates Group 1 [Fructose, Glucose, Sucrose]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#CARBS2", "Soluble Carbohydrates Group 2 [Fructose, Glucose, Sucrose, Sorbitol]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#TCARB", "Carbohydrate Panel [TNC, Starch, Fructose, Glucose, Sucrose, Total Glucose]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#FEED1", "Feed Group 1 [DM, Protein, ADF, TDN]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#FEED2", "Feed Group 2 [DM, Protein, ADF, TDN, Ash, Fat]", TestCategories.Plant, "DISCOUNTED GROUPS: ");

            CreateTestItem("C-P", "C", TestCategories.Plant, "TOTALS:");
            CreateTestItem("N-P", "N", TestCategories.Plant, "TOTALS:");
            CreateTestItem("TKN-P", "TKN", TestCategories.Plant, "TOTALS:");

            CreateTestItem("P-TOT", "P", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("K-TMW", "K", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("S-TOT", "S", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("B-PMF", "B", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("CA-PMF", "Ca", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("MG-PMF", "Mg", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("NA-PMF", "Na", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("ZN-PM", "Zn", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("MN-PM", "Mn", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("FE-PM", "Fe", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("CU-PM", "Cu", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("BA-PMF", "Ba", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("CD", "Cd", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("CR", "Cr", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("CO", "Co", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("PB", "Pb", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("MO", "Mo", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("NI", "Ni", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("V", "V", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("SE", "Se", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("AS-PT", "As", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("CR-OXIDE", "Cr (oxide)", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("SI-%", "Si", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("AL", "Al", TestCategories.Plant, "ACID DIGESTIBLES:");

            CreateTestItem("NO3-P", "NO3-N", TestCategories.Plant, "EXTRACTABLES:");
            CreateTestItem("NH4-P", "NH4-N", TestCategories.Plant, "EXTRACTABLES:");
            CreateTestItem("K-TOT", "K", TestCategories.Plant, "EXTRACTABLES:");
            CreateTestItem("CL-P-IC", "Cl", TestCategories.Plant, "EXTRACTABLES:");
            CreateTestItem("PO4-P", "PO4-P", TestCategories.Plant, "EXTRACTABLES:");
            CreateTestItem("SO4-PM", "SO4-S", TestCategories.Plant, "EXTRACTABLES:");

            CreateTestItem("DM", "DM", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("DM55", "Partial DM (dried at 55OC)", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#PROT", "Protein", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("ADFRF", "ADF", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#ADFAF", "ADF (ash free)", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#TDNRF", "TDN", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#LIG-AF", "Lignin", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#ADIN", "ADIN", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("NDFRF", "NDF", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#NDFAF", "NDF (ash free)", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#CELLULOS", "Cellulose", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("HEMICELL", "Hemicellulose", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("ASH", "Ash", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("FAT", "Fat", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("FAT-R", "Fat (Rinse)", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("PHENOLS", "Total Phenols *", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("ASCORBIC", "Ascorbic Acid *", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("CAROTENE", "β-Carotene *", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("TOCOPH", "α-Tocopherol *", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#STARCH", "Starch", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#TNC", "TNC", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("FRUC", "Fructose", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("GLUC", "Glucose", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("SUCR", "Sucrose", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("SORB", "Sorbitol", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("TGLUC", "Total Glucose", TestCategories.Plant, "FEED TESTS:");

            CreateTestItem("PH-M", "pH (water 1:5)", TestCategories.Plant, "MANURE and COMPOST TESTS:");
            CreateTestItem("EC-M", "EC (water 1:5)", TestCategories.Plant, "MANURE and COMPOST TESTS:");


            CreateTestItem("MICRO", "Acid Digestion (for analysis by ICP-MS)", TestCategories.Plant, "OTHER SERVICES REQUESTED:");


            //Water
            CreateTestItem("#WSUIT", "Water Suitability Group 1 [pH, EC, SAR, Ca, Mg, Na, Cl, B, HCO3, CO3]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#WSUIT-2", "Water Suitability Group 2 [pH, EC, SAR, Ca, Mg, Na, Cl, B]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#AD-SALTS", "Acid Digestible Salts [K, Ca, Mg, Na]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#AD-HM", "Acid Digestible Heavy Metals [Cd, Cr, Pb, Ni]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#CRBBI", "Bicarbonate & Carbonate [HCO3, CO3]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#IC-PANEL", "Ion Chromatography Panel [Cl, SO4]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#ANIONS", "Anion Panel [Cl, SO4-S (soluble S), NO3-N, HCO3]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#SLSLT", "Soluble Salts [K, Ca, Mg, Na]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#MICRS", "Soluble Micronutrients [Zn, Mn, Fe, Cu]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#SOL-HM", "Soluble Heavy Metals [Cd, Cr, Pb, Ni]", TestCategories.Water, "DISCOUNTED GROUPS:");

            CreateTestItem("PH-W", "pH", TestCategories.Water, "???");
            CreateTestItem("EC-W", "EC", TestCategories.Water, "???");
            CreateTestItem("TC-W", "Total C", TestCategories.Water, "???");
            CreateTestItem("TOC-W", "TOC", TestCategories.Water, "???");
            CreateTestItem("DOC-W", "DOC", TestCategories.Water, "???", "DOC-WF for unfiltered samples");
            CreateTestItem("TN-W", "Total N", TestCategories.Water, "???");
            CreateTestItem("TKN-W", "TKN", TestCategories.Water, "???");
            CreateTestItem("#SAR-W", "SAR", TestCategories.Water, "???");
            CreateTestItem("#ESP-W", "ESP", TestCategories.Water, "???");
            CreateTestItem("#HARD", "Hardness", TestCategories.Water, "???");
            CreateTestItem("CL-W", "Cl", TestCategories.Water, "???");
            CreateTestItem("HCO3-W", "HCO3", TestCategories.Water, "???");
            CreateTestItem("CO3-W", "CO3", TestCategories.Water, "???");
            CreateTestItem("P-W", "PO4-P (soluble P)", TestCategories.Water, "???");
            CreateTestItem("TDS", "TDS", TestCategories.Water, "???");
            CreateTestItem("TS", "TS", TestCategories.Water, "???");
            CreateTestItem("TSS", "TSS", TestCategories.Water, "???");
            CreateTestItem("VSS", "VSS", TestCategories.Water, "???", "requires TSS");
            CreateTestItem("ALK", "Alkalinity", TestCategories.Water, "???");
            CreateTestItem("TURBID", "Turbidity", TestCategories.Water, "???");

            CreateTestItem("K-SOLW", "K", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("SO4-W", "SO4-S (soluble S)", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("CA-W", "Ca", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("MG-W", "Mg", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("NA-W", "Na", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("B-W	 ", "B", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("ZN-W", "Zn", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("MN-W", "Mn", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("FE-W", "Fe", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("CU-W", "Cu", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("AL-W", "Al", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("SI-W", "Si", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("V-W	 ", "V", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("CD-W", "Cd", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("CR-W", "Cr", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("PB-W", "Pb", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("NI-W", "Ni", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("CL-WX", "Cl in mg/L", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("CA-WX", "Ca in mg/L", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("MG-WX", "Mg in mg/L", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("NA-WX", "Na in mg/L", TestCategories.Water, "SOLUBLE MINERALS:");

            CreateTestItem("P-WT", "P", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("K-WT", "K", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("S-WT", "S", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("CA-WT", "Ca", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("MG-WT", "Mg", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("NA-WT", "Na", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("B-WT", "B", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("ZN-WT", "Zn", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("MN-WT", "Mn", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("FE-WT", "Fe", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("CU-WT", "Cu", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("CD-WT", "Cd", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("CR-WT", "Cr", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("PB-WT", "Pb", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("MO-WT", "Mo", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("NI-WT", "Ni", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("HG-WT", "Hg", TestCategories.Water, "ACID DIGESTIBLE MINERALS:", "DO NOT USE");
            CreateTestItem("AL-WT", "Al", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("SE-W", "Se", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("AS-WT", "As", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");

            CreateTestItem("SO4-W-IC", "SO4 (Ion Chromatography)", TestCategories.Water, "ALTERNATE METHODS:");
            CreateTestItem("CL-W-IC", "Cl (Ion Chromatography)", TestCategories.Water, "ALTERNATE METHODS:");
            CreateTestItem("P-W-ICP", "Cl (Ion Chromatography)", TestCategories.Water, "ALTERNATE METHODS:");
        }

        private void CreateTestItem(string code, string analysis, string category, string group, string notes = null, bool pub = true)
        {
            var testItem = new TestItem
            {
                Id = code,
                Analysis = analysis,
                Category = category,
                Group = group,
                Notes = notes,
                Public = pub,
                LabOrder = TestItemCount,
                RequestOrder = TestItemCount
            };
            _context.Add(testItem);
            TestItemCount += 100;
        }
    }

}
