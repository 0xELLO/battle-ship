# DB demo


~~~bash
dotnet aspnet-codegenerator razorpage -m GameConfig -dc ApplicationDbContext -udl -outDir Pages/GameConfiguration --referenceScriptLibraries
dotnet aspnet-codegenerator razorpage -m GameShipConfig -dc ApplicationDbContext -udl -outDir Pages/ShipConfiguration --referenceScriptLibraries
~~~