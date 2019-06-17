﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetGoLynx.Data;

namespace NetGoLynx.Migrations
{
    [DbContext(typeof(RedirectContext))]
    [Migration("20190617051216_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("NetGoLynx.Models.Redirect", b =>
                {
                    b.Property<int>("RedirectId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Target");

                    b.HasKey("RedirectId");

                    b.ToTable("Redirects");
                });
#pragma warning restore 612, 618
        }
    }
}