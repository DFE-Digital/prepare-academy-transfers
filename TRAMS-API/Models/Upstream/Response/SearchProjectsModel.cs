using API.Models.Upstream.Enums;
using System;

namespace API.Models.Upstream.Response
{
    public class SearchProjectsModel
    {
        /// <summary>
        /// The Id of the Academy Transfers project
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// The name of the project
        /// </summary>
        /// <example>AT-10000</example>
        public string ProjectName { get; set; }

        /// <summary>
        /// The project initiator's full name
        /// </summary>
        /// <example>Joe Bloggs</example>
        public string ProjectInitiatorFullName { get; set; }

        /// <summary>
        /// The project initiator's unique id
        /// </summary>
        /// <example>joe.bloggs@email.com</example>
        public string ProjectInitiatorUid { get; set; }

        /// <summary>
        /// The status of the project
        /// </summary>
        public ProjectStatusEnum ProjectStatus { get; set; }
    }
}