using BikeRental.Data.ViewModels;
using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Voucher
    {
        public Voucher()
        {
            VoucherItems = new HashSet<VoucherItem>();
        }

        public Voucher(VoucherViewModel campaignViewModel)
        {
            Id = campaignViewModel.Id;
            Name = campaignViewModel.Name;
            Description = campaignViewModel.Description;
            DiscountPercent = campaignViewModel.DiscountPercent;
            DiscountAmount = campaignViewModel.DiscountAmount;
            IsAvailable = campaignViewModel.IsAvailable;
            ExpiredDate = campaignViewModel.ExpiredDate;
            StartingDate = campaignViewModel.StartingDate;
            VoucherItemsRemain = campaignViewModel.VoucherItemsRemain;
            CampaignId = campaignViewModel.CampaignId;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime StartingDate { get; set; }
        public int? VoucherItemsRemain { get; set; }
        public Guid? CampaignId { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual ICollection<VoucherItem> VoucherItems { get; set; }
    }
}
