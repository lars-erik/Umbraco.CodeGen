namespace Umbraco.CodeGen.Models
{
    using global::System;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Web;
    
    public partial interface IMixin : IPublishedContent
    {
        Int32 MixinProp
        {
            get;
        }
    }
}
