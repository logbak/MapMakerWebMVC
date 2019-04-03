using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Data
{

    // not being used currently - may reference later then delete
    public class ApplicationDbContextInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var maps = new List<Map>
            {
            new Map{Name="Map 1",Description="A map for maps.",SizeX=20,SizeY=20}

            };

            maps.ForEach(s => context.Maps.Add(s));
            context.SaveChanges();
        }
    }
}
