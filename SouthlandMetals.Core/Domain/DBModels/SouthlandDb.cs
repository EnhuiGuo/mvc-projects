using System;
using System.Data.Entity;
using System.Web;

namespace SouthlandMetals.Core.Domain.DBModels
{
    public class SouthlandDb : DbContext
    {
        public SouthlandDb() : base("name = SouthlandDb")
        {

        }

        public DbSet<AccountCode> AccountCode { get; set; }
        public DbSet<BillOfLading> BillOfLading { get; set; }
        public DbSet<Bucket> Bucket { get; set; }
        public DbSet<Carrier> Carrier { get; set; }
        public DbSet<CoatingType> CoatingType { get; set; }
        public DbSet<Container> Container { get; set; }
        public DbSet<ContainerPart> ContainerPart { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<CreditMemo> CreditMemo { get; set; }
        public DbSet<CreditMemoItem> CreditMemoItem { get; set; }
        public DbSet<CustomerOrder> CustomerOrder { get; set; }
        public DbSet<CustomerOrderPart> CustomerOrderPart { get; set; }
        public DbSet<DebitMemo> DebitMemo { get; set; }
        public DbSet<DebitMemoAttachment> DebitMemoAttachment { get; set; }
        public DbSet<DebitMemoItem> DebitMemoItem { get; set; }
        public DbSet<DebitMemoNumber> DebitMemoNumber { get; set; }
        public DbSet<Destination> Destination { get; set; }
        public DbSet<FoundryInvoice> FoundryInvoice { get; set; }
        public DbSet<FoundryOrder> FoundryOrder { get; set; }
        public DbSet<FoundryOrderPart> FoundryOrderPart { get; set; }
        public DbSet<HtsNumber> HtsNumber { get; set; }
        public DbSet<Material> Material { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<OrderTerm> OrderTerm { get; set; }
        public DbSet<PackingList> PackingList { get; set; }
        public DbSet<PackingListPart> PackingListPart { get; set; }
        public DbSet<Pallet> Pallet { get; set; }
        public DbSet<PalletPart> PalletPart { get; set; }
        public DbSet<Part> Part { get; set; }   
        public DbSet<PartDrawing> PartDrawing { get; set; }
        public DbSet<PartLayout> PartLayout { get; set; }
        public DbSet<PartStatus> PartStatus { get; set; }
        public DbSet<PartType> PartType { get; set; }
        public DbSet<PatternMaterial> PatternMaterial { get; set; }
        public DbSet<PaymentTerm> PaymentTerm { get; set; }
        public DbSet<Port> Port { get; set; }
        public DbSet<PriceSheet> PriceSheet { get; set; }
        public DbSet<PriceSheetBucket> PriceSheetBucket { get; set; }
        public DbSet<PriceSheetPart> PriceSheetPart { get; set; }
        public DbSet<PriceSheetNumber> PriceSheetNumber { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectNote> ProjectNote { get; set; }
        public DbSet<ProjectPart> ProjectPart { get; set; }
        public DbSet<ProjectPartDrawing> ProjectPartDrawing { get; set; }
        public DbSet<ProjectPartLayout> ProjectPartLayout { get; set; }
        public DbSet<PurchaseOrderNumber> PurchaseOrderNumber { get; set; }
        public DbSet<Quote> Quote { get; set; }
        public DbSet<QuoteNumber> QuoteNumber { get; set; }
        public DbSet<QuotePart> QuotePart { get; set; }
        public DbSet<Rfq> Rfq { get; set; }
        public DbSet<RfqNumber> RfqNumber { get; set; }
        public DbSet<RfqPart> RfqPart { get; set; }
        public DbSet<ShipCodeNumber> ShipCodeNumber { get; set; }
        public DbSet<Shipment> Shipment { get; set; }
        public DbSet<ShipmentTerm> ShipmentTerm { get; set; }
        public DbSet<SpecificationMaterial> SpecificationMaterial { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<Surcharge> Surcharge { get; set; }
        public DbSet<TrackingCode> TrackingCode { get; set; }
        public DbSet<Vessel> Vessel { get; set; }
        public object PackingLists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                      .HasMany<Part>(s => s.Parts)
                      .WithMany(f => f.Projects)
                      .Map(fs =>
                      {
                          fs.MapLeftKey("ProjectReferenceId");
                          fs.MapRightKey("PartReferenceId");
                          fs.ToTable("ProjectPartReference");
                      });
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            foreach (var auditableEntity in ChangeTracker.Entries<IBaseEntity>())
            {
                if (auditableEntity.State == EntityState.Added ||
                    auditableEntity.State == EntityState.Modified)
                {
                    // implementation may change based on the useage scenario, this
                    // sample is for forma authentication.
                    string currentUser = HttpContext.Current.User.Identity.Name;

                    // modify updated date and updated by column for 
                    // adds of updates.
                    auditableEntity.Entity.ModifiedDate = DateTime.Now;
                    auditableEntity.Entity.ModifiedBy = currentUser;

                    // pupulate created date and created by columns for
                    // newly added record.
                    if (auditableEntity.State == EntityState.Added)
                    {
                        auditableEntity.Entity.CreatedDate = DateTime.Now;
                        auditableEntity.Entity.CreatedBy = currentUser;
                    }
                    else
                    {
                        // we also want to make sure that code is not inadvertly
                        // modifying created date and created by columns 
                        auditableEntity.Property(p => p.CreatedDate).IsModified = false;
                        auditableEntity.Property(p => p.CreatedBy).IsModified = false;
                    }
                }
            }

            return base.SaveChanges();
        }
    }
}
