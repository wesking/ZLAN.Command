using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ZL.CommandCore.Data
{
    public partial class ServiceContext : DbContext
    {
        public ServiceContext()
        {
        }

        public ServiceContext(DbContextOptions<ServiceContext> options)
            : base(options)
        {
        }

        public static string ConnectionString { get; set; }

        public virtual DbSet<SvrApiInfo> SvrApiInfo { get; set; }
        public virtual DbSet<SvrAppAuthorization> SvrAppAuthorization { get; set; }
        public virtual DbSet<SvrAppInfo> SvrAppInfo { get; set; }
        public virtual DbSet<SvrAppService> SvrAppService { get; set; }
        public virtual DbSet<SvrCommandSubscriber> SvrCommandSubscriber { get; set; }
        public virtual DbSet<SvrInvokeInfo> SvrInvokeInfo { get; set; }
        public virtual DbSet<SvrInvokeLog> SvrInvokeLog { get; set; }
        public virtual DbSet<SvrServiceInfo> SvrServiceInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SvrApiInfo>(entity =>
            {
                entity.ToTable("svr_api_info");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CacheTime)
                    .HasColumnName("CACHE_TIME")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CacheVersion)
                    .HasColumnName("CACHE_VERSION")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Key)
                    .HasColumnName("KEY")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<SvrAppAuthorization>(entity =>
            {
                entity.ToTable("svr_app_authorization");

                entity.HasIndex(e => e.AppId)
                    .HasName("ROLE_CODE");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AppId)
                    .HasColumnName("APP_ID")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AuthorizationCode)
                    .HasColumnName("AUTHORIZATION_CODE")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<SvrAppInfo>(entity =>
            {
                entity.ToTable("svr_app_info");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AppId)
                    .IsRequired()
                    .HasColumnName("APP_ID")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.AppSecret)
                    .IsRequired()
                    .HasColumnName("APP_SECRET")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Memo)
                    .HasColumnName("MEMO")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");
            });

            modelBuilder.Entity<SvrAppService>(entity =>
            {
                entity.HasKey(e => new { e.AppId, e.ServiceId })
                    .HasName("PRIMARY");

                entity.ToTable("svr_app_service");

                entity.Property(e => e.AppId)
                    .HasColumnName("APP_ID")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.ServiceId)
                    .HasColumnName("SERVICE_ID")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Parameter)
                    .HasColumnName("PARAMETER")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");
            });

            modelBuilder.Entity<SvrCommandSubscriber>(entity =>
            {
                entity.ToTable("svr_command_subscriber");

                entity.HasIndex(e => e.CommandKey)
                    .HasName("COMMAND_KEY");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CommandKey)
                    .HasColumnName("COMMAND_KEY")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.ExecutorConfig)
                    .HasColumnName("EXECUTOR_CONFIG")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ExecutorType)
                    .HasColumnName("EXECUTOR_TYPE")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Sort)
                    .HasColumnName("SORT")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<SvrInvokeInfo>(entity =>
            {
                entity.ToTable("svr_invoke_info");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Api)
                    .HasColumnName("API")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.AppId)
                    .HasColumnName("APP_ID")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndTime)
                    .HasColumnName("END_TIME")
                    .HasColumnType("datetime");

                entity.Property(e => e.Interval)
                    .HasColumnName("INTERVAL")
                    .HasColumnType("int(11)");

                entity.Property(e => e.NextInvokeTime)
                    .HasColumnName("NEXT_INVOKE_TIME")
                    .HasColumnType("datetime");

                entity.Property(e => e.RequestBody)
                    .HasColumnName("REQUEST_BODY")
                    .HasColumnType("text");

                entity.Property(e => e.RequestHeaders)
                    .HasColumnName("REQUEST_HEADERS")
                    .HasColumnType("text");

                entity.Property(e => e.ResponseData)
                    .HasColumnName("RESPONSE_DATA")
                    .HasColumnType("text");

                entity.Property(e => e.ResponseTime)
                    .HasColumnName("RESPONSE_TIME")
                    .HasColumnType("double(11,0)");

                entity.Property(e => e.ServiceId)
                    .HasColumnName("SERVICE_ID")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Url)
                    .HasColumnName("URL")
                    .HasColumnType("varchar(500)");
            });

            modelBuilder.Entity<SvrInvokeLog>(entity =>
            {
                entity.ToTable("svr_invoke_log");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Api)
                    .HasColumnName("API")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AppId)
                    .HasColumnName("APP_ID")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("datetime");

                entity.Property(e => e.InvokeId)
                    .HasColumnName("INVOKE_ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RequestBody)
                    .HasColumnName("REQUEST_BODY")
                    .HasColumnType("text");

                entity.Property(e => e.RequestHeaders)
                    .HasColumnName("REQUEST_HEADERS")
                    .HasColumnType("text");

                entity.Property(e => e.ResponseData)
                    .HasColumnName("RESPONSE_DATA")
                    .HasColumnType("text");

                entity.Property(e => e.ResponseTime)
                    .HasColumnName("RESPONSE_TIME")
                    .HasColumnType("double(11,0)");

                entity.Property(e => e.ServiceId)
                    .HasColumnName("SERVICE_ID")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Url)
                    .HasColumnName("URL")
                    .HasColumnType("varchar(500)");
            });

            modelBuilder.Entity<SvrServiceInfo>(entity =>
            {
                entity.HasKey(e => e.ServiceId)
                    .HasName("PRIMARY");

                entity.ToTable("svr_service_info");

                entity.Property(e => e.ServiceId)
                    .HasColumnName("SERVICE_ID")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Memo)
                    .HasColumnName("MEMO")
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.ServiceUrl)
                    .HasColumnName("SERVICE_URL")
                    .HasColumnType("varchar(255)");
            });
        }
    }
}
