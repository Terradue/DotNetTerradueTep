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
    /// Output
    /// </summary>
    [DataContract]
    public partial class Output : DataType, IEquatable<Output>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Output" /> class.
        /// </summary>
        /// <param name="id">id.</param>
        /// <param name="transmissionMode">transmissionMode.</param>
        public Output(string id = default(string),
                      TransmissionMode transmissionMode = default(TransmissionMode),
                      Format format = default(Format)) : base()
        {
            this.Id = id;
            this.TransmissionMode = transmissionMode;
        }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or Sets TransmissionMode
        /// </summary>
        [DataMember(Name = "transmissionMode", EmitDefaultValue = false)]
        public TransmissionMode TransmissionMode { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Output {\n");
            sb.Append("  ").Append(base.ToString().Replace("\n", "\n  ")).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  TransmissionMode: ").Append(TransmissionMode).Append("\n");
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
            return this.Equals(input as Output);
        }

        /// <summary>
        /// Returns true if Output instances are equal
        /// </summary>
        /// <param name="input">Instance of Output to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Output input)
        {
            if (input == null)
                return false;

            return base.Equals(input) &&
                (
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
                ) && base.Equals(input) &&
                (
                    this.TransmissionMode == input.TransmissionMode ||
                    (this.TransmissionMode != null &&
                    this.TransmissionMode.Equals(input.TransmissionMode))
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
                if (this.Id != null)
                    hashCode = hashCode * 59 + this.Id.GetHashCode();
                if (this.TransmissionMode != null)
                    hashCode = hashCode * 59 + this.TransmissionMode.GetHashCode();
                return hashCode;
            }
        }
    }
}
