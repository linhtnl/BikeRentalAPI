using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class ChoThueXeMayContext : DbContext
    {
        public ChoThueXeMayContext()
        {
        }

        public ChoThueXeMayContext(DbContextOptions<ChoThueXeMayContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Bike> Bikes { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Campaign> Campaigns { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<MotorType> MotorTypes { get; set; }
        public virtual DbSet<Owner> Owners { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PriceList> PriceLists { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<VoucherExchangeHistory> VoucherExchangeHistories { get; set; }
        public virtual DbSet<VoucherItem> VoucherItems { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("Area");

                entity.HasIndex(e => e.Name, "ARE_NAME")
                    .IsUnique();

                entity.HasIndex(e => e.PostalCode, "POSTALCODE")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name).HasMaxLength(200);
            });

            modelBuilder.Entity<Bike>(entity =>
            {
                entity.ToTable("Bike");

                entity.HasIndex(e => e.LicensePlate, "licensePlate")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Color)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'-')");

                entity.Property(e => e.ImgPath).HasMaxLength(255);

                entity.Property(e => e.LicensePlate)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('-')");

                entity.Property(e => e.ModelYear)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Bikes)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__Bike__CategoryId__2A4B4B5E");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Bikes)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK__Bike__OwnerId__29572725");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.DayRent).HasColumnType("date");

                entity.Property(e => e.DayReturnActual).HasColumnType("date");

                entity.Property(e => e.DayReturnExpected).HasColumnType("date");

                entity.Property(e => e.Price).HasColumnType("decimal(8, 0)");

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Bike)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.BikeId)
                    .HasConstraintName("FK__Booking__BikeId__4CA06362");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Booking__Custome__4BAC3F29");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK__Booking__OwnerId__4D94879B");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK__Booking__Payment__4E88ABD4");

                entity.HasOne(d => d.VoucherCodeNavigation)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.VoucherCode)
                    .HasConstraintName("FK__Booking__Voucher__4AB81AF0");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("Brand");

                entity.HasIndex(e => e.Name, "name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.ToTable("Campaign");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ExpiredDate).HasColumnType("date");

                entity.Property(e => e.StartingDate).HasColumnType("date");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Campaigns)
                    .HasForeignKey(d => d.AreaId)
                    .HasConstraintName("FK__Campaign__AreaId__38996AB5");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK__Category__Branch__22AA2996");

                entity.HasOne(d => d.MotorType)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(d => d.MotorTypeId)
                    .HasConstraintName("FK__Category__MotorT__339FAB6E");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BanCount).HasDefaultValueSql("((0))");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(200)
                    .HasDefaultValueSql("(N'-')");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('-')")
                    .IsFixedLength(true);

                entity.Property(e => e.RewardPoints).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("FK__Customer__AdminI__34C8D9D1");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Content).HasMaxLength(500);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Feedback)
                    .HasForeignKey<Feedback>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Feedback__Id__52593CB8");
            });

            modelBuilder.Entity<MotorType>(entity =>
            {
                entity.ToTable("MotorType");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.ToTable("Owner");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address)
                    .HasMaxLength(200)
                    .HasDefaultValueSql("(N'-')");

                entity.Property(e => e.BanTimes).HasDefaultValueSql("((0))");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(200)
                    .HasDefaultValueSql("(N'-')");

                entity.Property(e => e.ImgPath).HasMaxLength(255);

                entity.Property(e => e.Mail)
                    .HasMaxLength(255)
                    .HasColumnName("mail");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('-')")
                    .IsFixedLength(true);

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.Owners)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("FK__Owner__AdminId__173876EA");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Owners)
                    .HasForeignKey(d => d.AreaId)
                    .HasConstraintName("FK__Owner__AreaId__182C9B23");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payment");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ActionDate).HasColumnType("datetime");

                entity.Property(e => e.Amount).HasColumnType("decimal(8, 0)");

                entity.Property(e => e.Name).HasMaxLength(20);
            });

            modelBuilder.Entity<PriceList>(entity =>
            {
                entity.HasKey(e => new { e.AreaId, e.MotorTypeId })
                    .HasName("PK__PriceLis__EA20FB07DF926C41");

                entity.ToTable("PriceList");

                entity.Property(e => e.Price).HasColumnType("numeric(8, 0)");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.PriceLists)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PriceList__AreaI__4C6B5938");

                entity.HasOne(d => d.MotorType)
                    .WithMany(p => p.PriceLists)
                    .HasForeignKey(d => d.MotorTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PriceList__Motor__4D5F7D71");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report");

                entity.HasIndex(e => e.Id, "UQ__Report__3214EC06206794BF")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content).IsRequired();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Report)
                    .HasForeignKey<Report>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Report_Booking");
            });

            modelBuilder.Entity<TransactionHistory>(entity =>
            {
                entity.ToTable("TransactionHistory");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ActionDate).HasColumnType("datetime");

                entity.Property(e => e.Amount).HasColumnType("decimal(8, 0)");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.TransactionHistories)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK__Transacti__Booki__571DF1D5");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.TransactionHistories)
                    .HasForeignKey(d => d.WalletId)
                    .HasConstraintName("FK__Transacti__Walle__5629CD9C");
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.ToTable("Voucher");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.DiscountAmount)
                    .HasColumnType("decimal(8, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DiscountPercent).HasDefaultValueSql("((0))");

                entity.Property(e => e.ExpiredDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.StartingDate).HasColumnType("date");

                entity.Property(e => e.VoucherItemsRemain).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Campaign)
                    .WithMany(p => p.Vouchers)
                    .HasForeignKey(d => d.CampaignId)
                    .HasConstraintName("FK__Voucher__Campaig__3F466844");
            });

            modelBuilder.Entity<VoucherExchangeHistory>(entity =>
            {
                entity.ToTable("VoucherExchangeHistory");

                entity.HasIndex(e => e.VoucherCode, "UQ__VoucherE__7F0ABCA920BC7FF1")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ActionDate).HasColumnType("date");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.VoucherExchangeHistories)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__VoucherEx__Custo__5BE2A6F2");

                entity.HasOne(d => d.VoucherCodeNavigation)
                    .WithOne(p => p.VoucherExchangeHistory)
                    .HasForeignKey<VoucherExchangeHistory>(d => d.VoucherCode)
                    .HasConstraintName("FK__VoucherEx__Vouch__5CD6CB2B");
            });

            modelBuilder.Entity<VoucherItem>(entity =>
            {
                entity.ToTable("VoucherItem");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.VoucherItems)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__VoucherIt__Custo__4316F928");

                entity.HasOne(d => d.Voucher)
                    .WithMany(p => p.VoucherItems)
                    .HasForeignKey(d => d.VoucherId)
                    .HasConstraintName("FK__VoucherIt__Vouch__440B1D61");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("Wallet");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Balance)
                    .HasColumnType("decimal(8, 0)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BankId)
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.BankName)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MomoId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Wallet)
                    .HasForeignKey<Wallet>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Wallet__Id__1B0907CE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
