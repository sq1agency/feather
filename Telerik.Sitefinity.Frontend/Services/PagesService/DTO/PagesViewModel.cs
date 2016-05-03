using System.Collections.Generic;

namespace Telerik.Sitefinity.Frontend.Services.PagesService.DTO
{
    /// <summary>
    /// Represents the object that should be returned
    /// </summary>
    public class PagesViewModel
    {
        /// <summary>
        /// Gets or sets the page urls.
        /// </summary>
        /// <value>
        /// The page urls.
        /// </value>
        public IEnumerable<string> PageUrls { get; set; }
    }
}
