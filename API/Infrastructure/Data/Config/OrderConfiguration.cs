using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            //This means that the ShipToAddress is a complex type owned by the Order entity.
            builder.OwnsOne(o => o.ShipToAddress, a =>
            {
                a.WithOwner(); //specifies that the ShipToAddress entity is owned by the Order entity.
            });

            builder.Property(s => s.Status) //is used to configure the Status property of the Order entity.
                .HasConversion( //specifies a conversion for the Status property
                    o => o.ToString(), //converts the enum value to a string when saving to the database
                    o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o) //converts the string back to the enum value when reading from the database
                );
        }
    }
}
