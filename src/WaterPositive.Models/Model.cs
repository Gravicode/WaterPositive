﻿using ProtoBuf.Grpc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WaterPositive.Models
{
    #region helpers
    public class StorageObject
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string FileUrl { get; set; }
        public string ContentType { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? LastAccess { get; set; }
    }
    public class StorageSetting
    {
        public string EndpointUrl { get; set; } = "https://is3.cloudhost.id";
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; } = "USWest1";
        public string Bucket { get; set; }
        public string BaseUrl { get; set; }
        public bool Ssl { get; set; } = true;
        public StorageSetting()
        {

        }
        public StorageSetting(string Endpoint, string Accesskey, string Secretkey, string Region, string Bucket)
        {
            this.EndpointUrl = Endpoint;
            this.AccessKey = Accesskey;
            this.SecretKey = Secretkey;
            this.Region = Region;
            this.Bucket = Bucket;
            GenerateBaseUrl();
        }
        public void GenerateBaseUrl()
        {
            this.BaseUrl = EndpointUrl + "/{bucket}/{key}";
        }
    }
    public class CCTVImage
    {
        public string CctvName { get; set; }
        public byte[] ImageBytes { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    #endregion
    #region auth
    [DataContract]
    public class AuthenticationModel
    {
        [DataMember(Order = 1)]
        public string ApiKey { get; set; }
    }
    [DataContract]
    public class AuthenticationUserModel
    {
        [DataMember(Order = 1)]
        public string Username { get; set; }
        [DataMember(Order = 2)]
        public string Password { get; set; }
    }
    [DataContract]
    public class AuthenticatedUserModel
    {
        [DataMember(Order = 1)]
        public string Username { get; set; }
        [DataMember(Order = 2)]
        public string AccessToken { get; set; }
        [DataMember(Order = 3)]
        public string TokenType { get; set; }
        [DataMember(Order = 4)]
        public DateTime? ExpiredDate { get; set; }
    }
    #endregion
    #region GRPC
    [ServiceContract]
    public interface IAuth
    {
        [OperationContract]
        Task<AuthenticatedUserModel> AuthenticateWithUsername(AuthenticationUserModel data, CallContext context = default);

        [OperationContract]
        Task<AuthenticatedUserModel> AuthenticateWithApiKey(AuthenticationModel data, CallContext context = default);
    }
    [ServiceContract]
    public interface ICCTV : ICrudGrpc<CCTV>
    {

    }
    [ServiceContract]
    public interface IWaterDepot : ICrudGrpc<WaterDepot>
    {

    }
    [ServiceContract]
    public interface IDataCounter : ICrudGrpc<DataCounter>
    {

    }

    [ServiceContract]
    public interface IWaterUsage : ICrudGrpc<WaterUsage>
    {

    } 
    
    [ServiceContract]
    public interface ISensorData : ICrudGrpc<SensorData>
    {

    }

    [ServiceContract]
    public interface IUserProfile : ICrudGrpc<UserProfile>
    {
        [OperationContract]
        Task<UserProfile> GetItemByEmail(InputCls input, CallContext context = default);

        [OperationContract]
        Task<UserProfile> GetItemByPhone(InputCls input, CallContext context = default);

        [OperationContract]
        Task<OutputCls> IsUserExists(InputCls input, CallContext context = default);

        [OperationContract]
        Task<OutputCls> GetUserRole(InputCls input, CallContext context = default);
    }
    #endregion

    #region Common
    public interface ICrud<T> where T : class
    {
        Task<bool> InsertData(T data);
        Task<bool> UpdateData(T data);
        Task<List<T>> GetAllData();
        Task<T> GetDataById(long Id);
        Task<bool> DeleteData(long Id);
        Task<long> GetLastId();
        Task<List<T>> FindByKeyword(string Keyword);
    }
    [ServiceContract]
    public interface ICrudGrpc<T> where T : class
    {
        [OperationContract]
        Task<OutputCls> InsertData(T data, CallContext context = default);
        [OperationContract]
        Task<OutputCls> UpdateData(T data, CallContext context = default);
        [OperationContract]
        Task<List<T>> GetAllData(CallContext context = default);
        [OperationContract]
        Task<T> GetDataById(InputCls Id, CallContext context = default);
        [OperationContract]
        Task<OutputCls> DeleteData(InputCls Id, CallContext context = default);
        [OperationContract]
        Task<OutputCls> GetLastId(CallContext context = default);
        [OperationContract]
        Task<List<T>> FindByKeyword(string Keyword, CallContext context = default);
    }
    [DataContract]
    public class InputCls
    {
        [DataMember(Order = 1)]
        public string[] Param { get; set; }
        [DataMember(Order = 2)]
        public Type[] ParamType { get; set; }
    }
    [DataContract]
    public class OutputCls
    {
        [DataMember(Order = 1)]
        public bool Result { get; set; }
        [DataMember(Order = 2)]
        public string Message { get; set; }
        [DataMember(Order = 3)]
        public string Data { get; set; }
    }
    #endregion
    #region database

    [DataContract]
    public class WaterDepot
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public string? Nama { get; set; }
        [DataMember(Order = 3)]
        public DateTime? TanggalPasang { get; set; }
        [DataMember(Order = 4)]
        public string? Lokasi { get; set; }

        [DataMember(Order = 5)]
        public string? Keterangan { get; set; }
        [DataMember(Order = 6)]
        public string? Latitude { get; set; }
        [DataMember(Order = 7)]
        public string? Longitude { get; set; }
        [InverseProperty(nameof(WaterUsage.WaterDepot))]
        public ICollection<WaterUsage> WaterUsages { get; set; }
    }

    [DataContract]
    public class CCTV
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public string? Nama { get; set; }
        [DataMember(Order = 3)]
        public DateTime? TanggalPasang { get; set; }
        [DataMember(Order = 4)]
        public string? Lokasi { get; set; }
        [DataMember(Order = 5)]
        public string? WaterDepot { get; set; }
        [DataMember(Order = 6)]
        public string? Merek { get; set; }
        [DataMember(Order = 7)]
        public string? Latitude { get; set; }
        [DataMember(Order = 8)]
        public string? Longitude { get; set; }
    }

    [DataContract]
    public class DataCounter
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public DateTime Tanggal { get; set; }
        [DataMember(Order = 3)]
        [MaxLength(100)]
        public string? Objek { get; set; }
        [DataMember(Order = 4)]
        [MaxLength(250)]
        public string? Lokasi { get; set; }
        [DataMember(Order = 5)]
        [MaxLength(100)]
        public string? CCTV { get; set; }
        [DataMember(Order = 6)]
        [MaxLength(150)]
        public string? Aktivitas { get; set; }
        [DataMember(Order = 7)]
        [MaxLength(150)]
        public string? Deskripsi { get; set; }
        [DataMember(Order = 8)]
        [MaxLength(300)]
        public string? FileUrl { get; set; }
        [DataMember(Order = 9)]
        [MaxLength(300)]
        public string? Tags { get; set; }
    }
    [DataContract]
    public class SensorData
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public DateTime Tanggal { get; set; }
        [DataMember(Order = 3)]
        public double Temperature { get; set; }
        [DataMember(Order = 4)]
        public double Tds { get; set; }
        [DataMember(Order = 5)]
        public double DO { get; set; }
        [DataMember(Order = 6)]
        public double Ph { get; set; }
       
        [DataMember(Order = 7)]
        public double Pressure { get; set; }
        [DataMember(Order = 8)]
        public double WaterLevel { get; set; }

        [DataMember(Order = 9)]
        public string DeviceId { get; set; }

        [DataMember(Order = 10)]
        [ForeignKey(nameof(WaterDepot))]
        public long WaterDepotId { get; set; }
        [DataMember(Order = 11)]
        public WaterDepot WaterDepot { get; set; }


    }
    [DataContract]
    public class UserProfile
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public string Username { get; set; }
        [DataMember(Order = 3)]
        public string Password { get; set; }
        [DataMember(Order = 4)]
        public string FullName { get; set; }
        [DataMember(Order = 5)]
        public string? Phone { get; set; }
        [DataMember(Order = 6)]
        public string? Email { get; set; }
        [DataMember(Order = 7)]
        public string? Alamat { get; set; }
        [DataMember(Order = 8)]
        public string? KTP { get; set; }
        [DataMember(Order = 9)]
        public string? PicUrl { get; set; }

        [DataMember(Order = 10)]
        public string UID { get; set; }
        [DataMember(Order = 11)]
        public string PIN { get; set; }

        [DataMember(Order = 12)]
        public bool Aktif { get; set; } = true;

        [DataMember(Order = 13)]
        public Roles Role { set; get; } = Roles.User;
        [InverseProperty(nameof(WaterUsage.User))]
        public ICollection<WaterUsage> WaterUsages { get; set; }

    }
    [DataContract]
    public class WaterUsage
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public UserProfile User { set; get; }

        [DataMember(Order = 3)]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        
        [DataMember(Order = 4)]
        public double Volume { get; set; }

        [DataMember(Order = 5)]
        public DateTime? Tanggal { get; set; }

        [DataMember(Order = 6)]
        [ForeignKey(nameof(WaterDepot))]
        public long  WaterDepotId { get; set; }
        [DataMember(Order = 7)]
        public WaterDepot WaterDepot { get; set; }

    }
    public enum Roles { Admin, User, Operator }
    [DataContract]
    public class WaterPrice
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public string Periode { get; set; }
        [DataMember(Order = 3)]
        public DateTime? TanggalAwal { get; set; } = DateTime.Today;
        [DataMember(Order = 4)]
        public DateTime? TanggalAkhir { get; set; } = new DateTime(DateTime.Today.Year,12,31);
        [DataMember(Order = 5)]
        public double PricePerLiter { get; set; } = 0;

        [DataMember(Order = 6)]
        public string? Keterangan { get; set; }
        
        [DataMember(Order = 7)]
        public string UpdatedBy { get; set; }
        
        [DataMember(Order = 8)]
        public DateTime? UpdatedDate { get; set; }

    }
    #endregion
}