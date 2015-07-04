// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlushType.cs" company="XamlNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// <summary>
// http://www.sharpgis.net/post/2011/08/28/GZIP-Compressed-Web-Requests-in-WP7-Take-2.aspx
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WP7Contrib.Communications.Compression
{
    internal enum FlushType
    {
        None,
        Partial,
        Sync,
        Full,
        Finish,
    }
}