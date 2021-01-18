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
        public string ProjectName { get; set; }

        /// <summary>
        /// The project initiator's full name
        /// </summary>
        public string ProjectInitiatorFullName { get; set; }

        /// <summary>
        /// The project initiator's unique id
        /// </summary>
        public string ProjectInitiatorUid { get; set; }

        /// <summary>
        /// The status of the project
        /// </summary>
        public ProjectStatusEnum ProjectStatus { get; set; }
    }
}