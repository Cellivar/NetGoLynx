﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using NetGoLynx.Data;

namespace NetGoLynx.Migrations
{
#pragma warning disable 1591
    [DbContext(typeof(RedirectContext))]
    [Migration("20190619050328_AddDescription")]
    partial class AddDescription
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

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<string>("Target");

                    b.HasKey("RedirectId");

                    b.HasIndex("Name");

                    b.ToTable("Redirects");
                });
#pragma warning restore 612, 618
        }
    }
#pragma warning restore 1591
}
