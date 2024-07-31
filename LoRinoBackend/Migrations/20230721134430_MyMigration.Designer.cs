﻿// <auto-generated />
using System;
using LoRinoBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LoRinoBackend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230721134430_MyMigration")]
    partial class MyMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("LoRinoBackend.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasColumnType("longtext");

                    b.Property<int?>("CompanyId")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Country")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<string>("Streeet")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("LoRinoBackend.Models.Cluster", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ClusterId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Cluster");
                });

            modelBuilder.Entity("LoRinoBackend.Models.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PhotoPath")
                        .HasColumnType("longtext");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("LoRinoBackend.Models.DecodedData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<long>("Energy")
                        .HasColumnType("bigint");

                    b.Property<double>("FlowRate")
                        .HasColumnType("double");

                    b.Property<long>("FwdTemp")
                        .HasColumnType("bigint");

                    b.Property<int?>("LoRaDataId")
                        .HasColumnType("int");

                    b.Property<long>("Power")
                        .HasColumnType("bigint");

                    b.Property<long>("RetTemp")
                        .HasColumnType("bigint");

                    b.Property<double>("Volume")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.HasIndex("LoRaDataId");

                    b.ToTable("DecodedData");
                });

            modelBuilder.Entity("LoRinoBackend.Models.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("CompanyId")
                        .HasColumnType("int");

                    b.Property<string>("DevEui")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int?>("DeviceTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("Lat")
                        .HasColumnType("double");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<double>("Long")
                        .HasColumnType("double");

                    b.Property<double>("MaxZoom")
                        .HasColumnType("double");

                    b.Property<double>("MinZoom")
                        .HasColumnType("double");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("DeviceTypeId");

                    b.HasIndex("LocationId");

                    b.ToTable("Device");
                });

            modelBuilder.Entity("LoRinoBackend.Models.DeviceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PhotoPath")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("DeviceType");
                });

            modelBuilder.Entity("LoRinoBackend.Models.EndDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("ClusterDataId")
                        .HasColumnType("int");

                    b.Property<string>("DevAddr")
                        .HasColumnType("longtext");

                    b.Property<string>("DevEui")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("ClusterDataId");

                    b.ToTable("EndDevice");
                });

            modelBuilder.Entity("LoRinoBackend.Models.EventTagLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Action")
                        .HasColumnType("longtext");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<string>("EventTagBy")
                        .HasColumnType("longtext");

                    b.Property<long>("EventTagTime")
                        .HasColumnType("bigint");

                    b.Property<int>("MoveeEventTagId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("EventTagLog");
                });

            modelBuilder.Entity("LoRinoBackend.Models.GwInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Altitude")
                        .HasColumnType("int");

                    b.Property<int>("Channel")
                        .HasColumnType("int");

                    b.Property<string>("GwEui")
                        .HasColumnType("longtext");

                    b.Property<double>("Latitude")
                        .HasColumnType("double");

                    b.Property<int?>("LoRaDataId")
                        .HasColumnType("int");

                    b.Property<double>("Longitude")
                        .HasColumnType("double");

                    b.Property<int>("RadioId")
                        .HasColumnType("int");

                    b.Property<string>("RfRegion")
                        .HasColumnType("longtext");

                    b.Property<int>("Rssi")
                        .HasColumnType("int");

                    b.Property<double>("Snr")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.HasIndex("LoRaDataId");

                    b.ToTable("GwInfo");
                });

            modelBuilder.Entity("LoRinoBackend.Models.LobaroDataFrame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Battery")
                        .HasColumnType("int");

                    b.Property<string>("DevEui")
                        .HasColumnType("longtext");

                    b.Property<long>("RecvTime")
                        .HasColumnType("bigint");

                    b.Property<int>("TemperatureFlow")
                        .HasColumnType("int");

                    b.Property<int>("Volume")
                        .HasColumnType("int");

                    b.Property<int>("VolumeFlow")
                        .HasColumnType("int");

                    b.Property<string>("WaterMeterEui")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("LobaroDataFrame");
                });

            modelBuilder.Entity("LoRinoBackend.Models.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<double>("Lat")
                        .HasColumnType("double");

                    b.Property<double>("Long")
                        .HasColumnType("double");

                    b.Property<double>("MapZoom")
                        .HasColumnType("double");

                    b.Property<double>("MaxZoom")
                        .HasColumnType("double");

                    b.Property<double>("MinZoom")
                        .HasColumnType("double");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Road")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RoadSection")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("TimerLenght")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("LoRinoBackend.Models.LocationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.HasIndex("UserId");

                    b.ToTable("LocationUser");
                });

            modelBuilder.Entity("LoRinoBackend.Models.LoRaData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Adr")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("ClassB")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("CodingRate")
                        .HasColumnType("longtext");

                    b.Property<bool>("Confirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("DataRate")
                        .HasColumnType("longtext");

                    b.Property<bool>("Delayed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Encrypted")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("EndDeviceDataId")
                        .HasColumnType("int");

                    b.Property<int>("FCntDown")
                        .HasColumnType("int");

                    b.Property<int>("FCntUp")
                        .HasColumnType("int");

                    b.Property<int>("FPort")
                        .HasColumnType("int");

                    b.Property<int>("GwCnt")
                        .HasColumnType("int");

                    b.Property<string>("Modulation")
                        .HasColumnType("longtext");

                    b.Property<string>("MsgId")
                        .HasColumnType("longtext");

                    b.Property<string>("Payload")
                        .HasColumnType("longtext");

                    b.Property<long>("RecvTime")
                        .HasColumnType("bigint");

                    b.Property<float>("UlFrequency")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("EndDeviceDataId");

                    b.ToTable("LoraData");
                });

            modelBuilder.Entity("LoRinoBackend.Models.MoveeDataFrame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AckId")
                        .HasColumnType("longtext");

                    b.Property<bool>("AckMsg")
                        .HasColumnType("tinyint(1)");

                    b.Property<long>("AckTime")
                        .HasColumnType("bigint");

                    b.Property<double>("Battery")
                        .HasColumnType("double");

                    b.Property<int>("DataType")
                        .HasColumnType("int");

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<string>("Guid")
                        .HasColumnType("longtext");

                    b.Property<int>("Gx")
                        .HasColumnType("int");

                    b.Property<int>("Gy")
                        .HasColumnType("int");

                    b.Property<int>("Gz")
                        .HasColumnType("int");

                    b.Property<int>("MoveeEventFrameId")
                        .HasColumnType("int");

                    b.Property<long>("RecvTime")
                        .HasColumnType("bigint");

                    b.Property<double>("Temperature")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("MoveeEventFrameId");

                    b.ToTable("MoveeDataFrame");
                });

            modelBuilder.Entity("LoRinoBackend.Models.MoveeEventComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("varchar(1000)");

                    b.Property<string>("EventCommentBy")
                        .HasColumnType("longtext");

                    b.Property<long>("EventCommentTime")
                        .HasColumnType("bigint");

                    b.Property<int>("MoveeEventFrameId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MoveeEventFrameId");

                    b.ToTable("MoveeEventComment");
                });

            modelBuilder.Entity("LoRinoBackend.Models.MoveeEventFrame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AckMessage")
                        .HasColumnType("longtext");

                    b.Property<int>("AlarmCount")
                        .HasColumnType("int");

                    b.Property<string>("ClearMessage")
                        .HasColumnType("longtext");

                    b.Property<string>("EventAckBy")
                        .HasColumnType("longtext");

                    b.Property<long>("EventAckTime")
                        .HasColumnType("bigint");

                    b.Property<string>("EventClearBy")
                        .HasColumnType("longtext");

                    b.Property<long>("EventClearTime")
                        .HasColumnType("bigint");

                    b.Property<long>("EventCreationTime")
                        .HasColumnType("bigint");

                    b.Property<string>("Guid")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsAcked")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsCleared")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<bool>("TimerIsEnded")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("MoveeEventFrame");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AckMessage = "DummyEvent",
                            AlarmCount = 0,
                            ClearMessage = "DummyEvent",
                            EventAckBy = "",
                            EventAckTime = 0L,
                            EventClearBy = "",
                            EventClearTime = 0L,
                            EventCreationTime = 0L,
                            Guid = "",
                            IsAcked = false,
                            IsCleared = false,
                            LocationId = 0,
                            TimerIsEnded = false
                        });
                });

            modelBuilder.Entity("LoRinoBackend.Models.MoveeEventTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("MoveeEventFrameId")
                        .HasColumnType("int");

                    b.Property<int>("MoveeTagId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MoveeEventFrameId");

                    b.HasIndex("MoveeTagId");

                    b.ToTable("MoveeEventTag");
                });

            modelBuilder.Entity("LoRinoBackend.Models.MoveeTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("MoveeTag");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("LoRinoBackend.Models.ApplicationUser", b =>
                {
                    b.HasOne("LoRinoBackend.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Company");
                });

            modelBuilder.Entity("LoRinoBackend.Models.DecodedData", b =>
                {
                    b.HasOne("LoRinoBackend.Models.LoRaData", "LoRaData")
                        .WithMany()
                        .HasForeignKey("LoRaDataId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("LoRaData");
                });

            modelBuilder.Entity("LoRinoBackend.Models.Device", b =>
                {
                    b.HasOne("LoRinoBackend.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LoRinoBackend.Models.DeviceType", "DeviceType")
                        .WithMany()
                        .HasForeignKey("DeviceTypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("LoRinoBackend.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Company");

                    b.Navigation("DeviceType");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("LoRinoBackend.Models.EndDevice", b =>
                {
                    b.HasOne("LoRinoBackend.Models.Cluster", "ClusterData")
                        .WithMany()
                        .HasForeignKey("ClusterDataId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ClusterData");
                });

            modelBuilder.Entity("LoRinoBackend.Models.GwInfo", b =>
                {
                    b.HasOne("LoRinoBackend.Models.LoRaData", null)
                        .WithMany("GwInfoData")
                        .HasForeignKey("LoRaDataId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("LoRinoBackend.Models.Location", b =>
                {
                    b.HasOne("LoRinoBackend.Models.Company", "Company")
                        .WithMany("Location")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Company");
                });

            modelBuilder.Entity("LoRinoBackend.Models.LocationUser", b =>
                {
                    b.HasOne("LoRinoBackend.Models.Location", null)
                        .WithMany("LocationUserList")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LoRinoBackend.Models.ApplicationUser", null)
                        .WithMany("LocationUserList")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("LoRinoBackend.Models.LoRaData", b =>
                {
                    b.HasOne("LoRinoBackend.Models.EndDevice", "EndDeviceData")
                        .WithMany()
                        .HasForeignKey("EndDeviceDataId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("EndDeviceData");
                });

            modelBuilder.Entity("LoRinoBackend.Models.MoveeDataFrame", b =>
                {
                    b.HasOne("LoRinoBackend.Models.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LoRinoBackend.Models.MoveeEventFrame", null)
                        .WithMany("MoveeDataFrames")
                        .HasForeignKey("MoveeEventFrameId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Device");
                });

            modelBuilder.Entity("LoRinoBackend.Models.MoveeEventComment", b =>
                {
                    b.HasOne("LoRinoBackend.Models.MoveeEventFrame", null)
                        .WithMany("MoveeEventComment")
                        .HasForeignKey("MoveeEventFrameId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("LoRinoBackend.Models.MoveeEventTag", b =>
                {
                    b.HasOne("LoRinoBackend.Models.MoveeEventFrame", null)
                        .WithMany("MoveeEventTag")
                        .HasForeignKey("MoveeEventFrameId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LoRinoBackend.Models.MoveeTag", null)
                        .WithMany("MoveeEventTag")
                        .HasForeignKey("MoveeTagId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("LoRinoBackend.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("LoRinoBackend.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("LoRinoBackend.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("LoRinoBackend.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("LoRinoBackend.Models.ApplicationUser", b =>
                {
                    b.Navigation("LocationUserList");
                });

            modelBuilder.Entity("LoRinoBackend.Models.Company", b =>
                {
                    b.Navigation("Location");
                });

            modelBuilder.Entity("LoRinoBackend.Models.Location", b =>
                {
                    b.Navigation("LocationUserList");
                });

            modelBuilder.Entity("LoRinoBackend.Models.LoRaData", b =>
                {
                    b.Navigation("GwInfoData");
                });

            modelBuilder.Entity("LoRinoBackend.Models.MoveeEventFrame", b =>
                {
                    b.Navigation("MoveeDataFrames");

                    b.Navigation("MoveeEventComment");

                    b.Navigation("MoveeEventTag");
                });

            modelBuilder.Entity("LoRinoBackend.Models.MoveeTag", b =>
                {
                    b.Navigation("MoveeEventTag");
                });
#pragma warning restore 612, 618
        }
    }
}
