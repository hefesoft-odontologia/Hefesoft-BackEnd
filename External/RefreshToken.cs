using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testJsonDynamic.External
{
    public class RefreshToken : TableEntity
    {
        
        private string _id;

        [Key]
        public string Id
        {
            get { return _id; }
            set 
            { 
                _id = value;
                PartitionKey = _id;
            }
        }
        

        [Required]
        [MaxLength(50)]
        public string Subject { get; set; }
        
        
        private string _clientId ;

        [Required]
        [MaxLength(50)]
        public string ClientId 
        {
            get { return _clientId ; }
            set { 
                //_clientId  = value;
                RowKey = "refresh";
            }
        }
        


        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        [Required]
        public string ProtectedTicket { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey  { get; set; }


        public RefreshToken()
        {
            
        }
    }
}
