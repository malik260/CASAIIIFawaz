using CASA3.Models;
using Core.DTOs;
using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CASA3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new HomePageVM();
            model.Banner = new List<BannerDto>
            {
                new BannerDto
                {
                    ImageUrl = "/images/banners/banner-1.jpg",
                    Tag = "PROJECT OF THE MONTH",
                    Title = "CAPRI ISLAND",
                    Subtitle = "For those who dream in color",
                    CtaText = "DOWNLOAD BROCHURE",
                    CtaUrl = "#"
                },
                new BannerDto
                {
                    ImageUrl = "/images/banners/banner-2.jpg",
                    Tag = "FEATURED DEVELOPMENT",
                    Title = "CASA III",
                    Subtitle = "Luxury meets serenity",
                    CtaText = "VIEW PROJECT",
                    CtaUrl = "#"
                }
            };

            // Featured Projects data
            model.FeaturedProjects = new List<FeaturedProjectDto>
            {
                new FeaturedProjectDto { ImageUrl = "/images/projects/project-1.webp", Title = "CAPRI ISLAND", Url = "/projects/capri" },
                new FeaturedProjectDto { ImageUrl = "/images/projects/project-2.webp", Title = "LANGKAWI", Url = "/projects/langkawi" },
                new FeaturedProjectDto { ImageUrl = "/images/projects/project-3.webp", Title = "MALDIVES", Url = "/projects/maldives" }
            };

            // Bilaad Footprint data
            model.FootprintYears = new List<FootprintYearDto>
            {
                new FootprintYearDto
                {
                    Year = 2018,
                    Projects = new List<FootprintProjectDto>
                    {
                        new FootprintProjectDto
                        {
                            Title = "BOBOWASI ISLAND",
                            Subtitle = "Pioneer project of Bobowasi Island",
                            ImageUrl = "/images/projects/project-1.webp",
                            Url = "/projects/bobowasi"
                        },
                        new FootprintProjectDto
                        {
                            Title = "BORABORA ISLAND",
                            Subtitle = "Pioneer project of Borabora Island",
                            ImageUrl = "/images/projects/project-2.webp",
                            Url = "/projects/borabora"
                        }
                    }
                },
                new FootprintYearDto { Year = 2019, Projects = new List<FootprintProjectDto>() },
                new FootprintYearDto { Year = 2020, Projects = new List<FootprintProjectDto>() },
                new FootprintYearDto { Year = 2021, Projects = new List<FootprintProjectDto>() },
                new FootprintYearDto { Year = 2022, Projects = new List<FootprintProjectDto>() },
                new FootprintYearDto
                {
                    Year = 2023,
                    Projects = new List<FootprintProjectDto>
                    {
                        new FootprintProjectDto
                        {
                            Title = "BOBOWASI ISLAND",
                            Subtitle = "Pioneer project of Bobowasi Island",
                            ImageUrl = "/images/projects/project-1.webp",
                            Url = "/projects/bobowasi"
                        },
                        new FootprintProjectDto
                        {
                            Title = "BORABORA ISLAND",
                            Subtitle = "Pioneer project of Borabora Island",
                            ImageUrl = "/images/projects/project-2.webp",
                            Url = "/projects/borabora"
                        },
                        new FootprintProjectDto
                        {
                            Title = "BOBOWASI ISLAND",
                            Subtitle = "Pioneer project of Bobowasi Island",
                            ImageUrl = "/images/projects/project-1.webp",
                            Url = "/projects/bobowasi"
                        },
                        new FootprintProjectDto
                        {
                            Title = "BORABORA ISLAND",
                            Subtitle = "Pioneer project of Borabora Island",
                            ImageUrl = "/images/projects/project-2.webp",
                            Url = "/projects/borabora"
                        }
                    }
                },
                new FootprintYearDto { Year = 2024, Projects = new List<FootprintProjectDto>() },
                new FootprintYearDto { Year = 2025, Projects = new List<FootprintProjectDto>() }
            };
            model.SelectedFootprintYear = 2018;

            // Newsletters data
            model.Newsletters = new List<NewsletterDto>
            {
                new NewsletterDto
                {
                    CoverImageUrl = "/images/Newsletter/Newsletter-Feb.-2026-Cover.jpg-scaled.webp",
                    PdfUrl = "#" // Replace with actual PDF URL
                },
                new NewsletterDto
                {
                    CoverImageUrl = "/images/Newsletter/Newsletter-Jan.-2026-Cover-scaled.webp",
                    PdfUrl = "#" // Replace with actual PDF URL
                },
                new NewsletterDto
                {
                    CoverImageUrl = "/images/Newsletter/COVER-1_page-0001-scaled.webp",
                    PdfUrl = "#" // Replace with actual PDF URL
                }
            };

            // Partners data - Set in ViewData for layout access
            ViewData["Partners"] = GetPartners();
            
            // Projects data for navigation dropdown - Set in ViewData for layout access
            ViewData["Projects"] = GetProjects();

            return View(model);
        }

        private List<ProjectDto> GetProjects()
        {
            return new List<ProjectDto>
            {
                new ProjectDto { Name = "AMAZON", Url = "/projects/amazon" },
                new ProjectDto { Name = "BAHAMAS", Url = "/projects/bahamas" },
                new ProjectDto { Name = "BIMINI", Url = "/projects/bimini" },
                new ProjectDto { Name = "BALI ISLAND", Url = "/projects/bali" },
                new ProjectDto { Name = "BARBADOS ISLAND", Url = "/projects/barbados" },
                new ProjectDto { Name = "BOBOWASI ISLAND", Url = "/projects/bobowasi" },
                new ProjectDto { Name = "BORA BORA ISLAND", Url = "/projects/borabora" },
                new ProjectDto { Name = "CAPRI ISLAND", Url = "/projects/capri" },
                new ProjectDto { Name = "FIJI ISLAND", Url = "/projects/fiji" },
                new ProjectDto { Name = "LANGKAWI ISLAND", Url = "/projects/langkawi" },
                new ProjectDto { Name = "MALDIVES", Url = "/projects/maldives" },
                new ProjectDto { Name = "MAURITIUS ISLAND", Url = "/projects/mauritius" },
                new ProjectDto { Name = "SEYCHELLES", Url = "/projects/seychelles" },
                new ProjectDto { Name = "ZANZIBAR", Url = "/projects/zanzibar" }
            };
        }

        private List<PartnerDto> GetPartners()
        {
            return new List<PartnerDto>
            {
                new PartnerDto
                {
                    Name = "Jaiz Bank",
                    ImageUrl = "/images/partners/jaiz-1-1.webp",
                    WebsiteUrl = "#"
                },
                new PartnerDto
                {
                    Name = "Metropolitan",
                    ImageUrl = "/images/partners/metro-2.webp",
                    WebsiteUrl = "#"
                },
                new PartnerDto
                {
                    Name = "Partner 3",
                    ImageUrl = "/images/partners/My-project-1-9-1-1.webp",
                    WebsiteUrl = "#"
                },
                new PartnerDto
                {
                    Name = "New Edge Bank",
                    ImageUrl = "/images/partners/117-1-1.webp",
                    WebsiteUrl = "#"
                },
                new PartnerDto
                {
                    Name = "Metropolitan",
                    ImageUrl = "/images/partners/metro-2.webp",
                    WebsiteUrl = "#"
                }
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
