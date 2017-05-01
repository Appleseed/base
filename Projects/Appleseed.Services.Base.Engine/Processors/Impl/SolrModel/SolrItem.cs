using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolrNet.Attributes;

namespace Appleseed.Services.Base.Engine.Processors.Impl.SolrModel
{
    class SolrItem
    {
        [SolrUniqueKey("id")]
        public string Key { get; set; }//
        // Item Identity Fields
        [SolrField("path")]
        public string Path { get; set; }// *
        [SolrField("name")]
        public string Name { get; set; }// *


        [SolrField("content")]
        public string Content { get; set; }// *
        [SolrField("summary")]
        public string Summary { get; set; }// *

        // Item Smart Content Fields
        [SolrField("smartKeywords")]
        public string SmartKeywords { get; set; }// *

        [SolrField("type")]
        public string Type { get; set; }//

        // Item Privacy Fields
        [SolrField("viewRole")]
        public string ViewRoles { get; set; }//
        [SolrField("portalID")]
        public int PortalID { get; set; }//
        [SolrField("moduleID")]
        public int ModuleID { get; set; }//
        [SolrField("pageID")]
        public int PageID { get; set; }//
        [SolrField("fileSize")]
        public long FileSize { get; set; }//


        // Item Content Fields
       

        [SolrField("createdDate")]
        public DateTime CreatedDate { get; set; }//

        [SolrField("source")]
        public string Source { get; set; }//


    }
}
