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
    /// StatusInfo
    /// </summary>
    [DataContract]
    public partial class StatusInfo : IEquatable<StatusInfo>
    {
        /// <summary>
        /// Defines Status
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum StatusEnum
        {
            /// <summary>
            /// Enum Accepted for value: accepted
            /// </summary>
            [EnumMember(Value = "accepted")]
            Accepted = 1,
            /// <summary>
            /// Enum Running for value: running
            /// </summary>
            [EnumMember(Value = "running")]
            Running = 2,
            /// <summary>
            /// Enum Successful for value: successful
            /// </summary>
            [EnumMember(Value = "successful")]
            Successful = 3,
            /// <summary>
            /// Enum Failed for value: failed
            /// </summary>
            [EnumMember(Value = "failed")]
            Failed = 4,
            /// <summary>
            /// Enum Dismissed for value: dismissed
            /// </summary>
            [EnumMember(Value = "dismissed")]
            Dismissed = 5
        }
        /// <summary>
        /// Gets or Sets Status
        /// </summary>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public StatusEnum Status { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusInfo" /> class.
        /// </summary>
        /// <param name="jobID">jobID (required).</param>
        /// <param name="status">status (required).</param>
        /// <param name="message">message.</param>
        /// <param name="progress">progress.</param>
        /// <param name="links">links.</param>
        public StatusInfo(string jobID = default(string), StatusEnum status = default(StatusEnum), string message = default(string), int? progress = default(int?), List<Link> links = default(List<Link>))
        {
            // to ensure "jobID" is required (not null)
            if (jobID == null)
            {
                throw new InvalidDataException("jobID is a required property for StatusInfo and cannot be null");
            }
            else
            {
                this.JobID = jobID;
            }
            // to ensure "status" is required (not null)
            if (status == null)
            {
                throw new InvalidDataException("status is a required property for StatusInfo and cannot be null");
            }
            else
            {
                this.Status = status;
            }
            this.Message = message;
            this.Progress = progress;
            this.Links = links;
        }

        /// <summary>
        /// Gets or Sets JobID
        /// </summary>
        [DataMember(Name = "jobID", EmitDefaultValue = false)]
        public string JobID { get; set; }


        /// <summary>
        /// Gets or Sets Message
        /// </summary>
        [DataMember(Name = "message", EmitDefaultValue = false)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or Sets created date time
        /// </summary>
        [DataMember(Name = "created", EmitDefaultValue = false)]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or Sets started date time
        /// </summary>
        [DataMember(Name = "started", EmitDefaultValue = false)]
        public DateTime Started { get; set; }

        /// <summary>
        /// Gets or Sets finished date time
        /// </summary>
        [DataMember(Name = "finished", EmitDefaultValue = false)]
        public DateTime Finished { get; set; }

        /// <summary>
        /// Gets or Sets updated date time
        /// </summary>
        [DataMember(Name = "updated", EmitDefaultValue = false)]
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or Sets Progress
        /// </summary>
        [DataMember(Name = "progress", EmitDefaultValue = false)]
        public int? Progress { get; set; }

        /// <summary>
        /// Gets or Sets Links
        /// </summary>
        [DataMember(Name = "links", EmitDefaultValue = false)]
        public List<Link> Links { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class StatusInfo {\n");
            sb.Append("  JobID: ").Append(JobID).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("  Progress: ").Append(Progress).Append("\n");
            sb.Append("  Links: ").Append(Links).Append("\n");
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
            return this.Equals(input as StatusInfo);
        }

        /// <summary>
        /// Returns true if StatusInfo instances are equal
        /// </summary>
        /// <param name="input">Instance of StatusInfo to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(StatusInfo input)
        {
            if (input == null)
                return false;

            return
                (
                    this.JobID == input.JobID ||
                    (this.JobID != null &&
                    this.JobID.Equals(input.JobID))
                ) &&
                (
                    this.Status == input.Status ||
                    (this.Status != null &&
                    this.Status.Equals(input.Status))
                ) &&
                (
                    this.Message == input.Message ||
                    (this.Message != null &&
                    this.Message.Equals(input.Message))
                ) &&
                (
                    this.Progress == input.Progress ||
                    (this.Progress != null &&
                    this.Progress.Equals(input.Progress))
                ) &&
                (
                    this.Links == input.Links ||
                    this.Links != null &&
                    input.Links != null &&
                    this.Links.SequenceEqual(input.Links)
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
                if (this.JobID != null)
                    hashCode = hashCode * 59 + this.JobID.GetHashCode();
                if (this.Status != null)
                    hashCode = hashCode * 59 + this.Status.GetHashCode();
                if (this.Message != null)
                    hashCode = hashCode * 59 + this.Message.GetHashCode();
                if (this.Progress != null)
                    hashCode = hashCode * 59 + this.Progress.GetHashCode();
                if (this.Links != null)
                    hashCode = hashCode * 59 + this.Links.GetHashCode();
                return hashCode;
            }
        }
    }
}
