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
    /// AnyValue
    /// </summary>
    [DataContract]
    public partial class AnyValue : IEquatable<AnyValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnyValue" /> class.
        /// </summary>
        /// <param name="anyValue">anyValue (default to true).</param>
        public AnyValue(bool? anyValue = true)
        {
            // use default value if no "anyValue" provided
            if (anyValue == null)
            {
                this._AnyValue = true;
            }
            else
            {
                this._AnyValue = anyValue;
            }
        }

        /// <summary>
        /// Gets or Sets _AnyValue
        /// </summary>
        [DataMember(Name = "anyValue", EmitDefaultValue = false)]
        public bool? _AnyValue { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AnyValue {\n");
            sb.Append("  _AnyValue: ").Append(_AnyValue).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
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
            return this.Equals(input as AnyValue);
        }

        /// <summary>
        /// Returns true if AnyValue instances are equal
        /// </summary>
        /// <param name="input">Instance of AnyValue to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AnyValue input)
        {
            if (input == null)
                return false;

            return
                (
                    this._AnyValue == input._AnyValue ||
                    (this._AnyValue != null &&
                    this._AnyValue.Equals(input._AnyValue))
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
                int hashCode = 41;
                if (this._AnyValue != null)
                    hashCode = hashCode * 59 + this._AnyValue.GetHashCode();
                return hashCode;
            }
        }
    }
}
