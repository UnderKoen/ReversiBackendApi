// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReversiRestApi.DAL;

namespace ReversiRestApi.Migrations
{
    [DbContext(typeof(ReversiContext))]
    [Migration("20210610194920_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.13");

            modelBuilder.Entity("ReversiRestApi.Model.Spel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AandeBeurt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BordAsString")
                        .HasColumnType("TEXT");

                    b.Property<string>("Omschrijving")
                        .HasColumnType("TEXT");

                    b.Property<string>("Speler1Token")
                        .HasColumnType("TEXT");

                    b.Property<string>("Speler2Token")
                        .HasColumnType("TEXT");

                    b.Property<string>("Token")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Spellen");
                });
#pragma warning restore 612, 618
        }
    }
}
