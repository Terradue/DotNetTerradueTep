/* 
 * The OGC API - Processes
 *
 * WARNING - THIS IS WORK IN PROGRESS
 *
 * OpenAPI spec version: 1.0-draft.5
 * Contact: b.pross@52north.org
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SwaggerDateConverter = IO.Swagger.Client.SwaggerDateConverter;

namespace IO.Swagger.Model
{
    /// <summary>
    /// FormatDescription
    /// </summary>
    [DataContract]
        public partial class FormatDescription : Format,  IEquatable<FormatDescription>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatDescription" /> class.
        /// </summary>
        /// <param name="maximumMegabytes">maximumMegabytes.</param>
        /// <param name="_default">_default (default to false).</param>
        public FormatDescription(int? maximumMegabytes = default(int?), bool? _default = false, string mimeType = default(string), string schema = default(string), string encoding = default(string)) : base()
        {
            this.MaximumMegabytes = maximumMegabytes;
            // use default value if no "_default" provided
            if (_default == null)
            {
                this.Default = false;
            }
            else
            {
                this.Default = _default;
            }
        }
        
        /// <summary>
        /// Gets or Sets MaximumMegabytes
        /// </summary>
        [DataMember(Name="maximumMegabytes", EmitDefaultValue=false)]
        public int? MaximumMegabytes { get; set; }

        /// <summary>
        /// Gets or Sets Default
        /// </summary>
        [DataMember(Name="default", EmitDefaultValue=false)]
        public bool? Default { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class FormatDescription {\n");
            sb.Append("  ").Append(base.ToString().Replace("\n", "\n  ")).Append("\n");
            sb.Append("  MaximumMegabytes: ").Append(MaximumMegabytes).Append("\n");
            sb.Append("  Default: ").Append(Default).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public override string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as FormatDescription);
        }

        /// <summary>
        /// Returns true if FormatDescription instances are equal
        /// </summary>
        /// <param name="input">Instance of FormatDescription to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(FormatDescription input)
        {
            if (input == null)
                return false;

            return base.Equals(input) && 
                (
                    this.MaximumMegabytes == input.MaximumMegabytes ||
                    (this.MaximumMegabytes != null &&
                    this.MaximumMegabytes.Equals(input.MaximumMegabytes))
                ) && base.Equals(input) && 
                (
                    this.Default == input.Default ||
                    (this.Default != null &&
                    this.Default.Equals(input.Default))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = base.GetHashCode();
                if (this.MaximumMegabytes != null)
                    hashCode = hashCode * 59 + this.MaximumMegabytes.GetHashCode();
                if (this.Default != null)
                    hashCode = hashCode * 59 + this.Default.GetHashCode();
                return hashCode;
            }
        }
    }
}
