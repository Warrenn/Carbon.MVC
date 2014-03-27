 
 

using System;
using CarbonKnown.DAL.Models;
using System.Data.Entity.Migrations;
namespace CarbonKnown.DAL.Migrations
{
    public  sealed partial class Configuration
	{
        private void GeneratedSeed(DataContext context)
        {
			context.Calculations.AddOrUpdate(
                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("9611eb6c-9bec-4846-b4f4-d53c0a3d4455")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.Accommodation.AccommodationCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("263129dc-29c2-40ed-8184-e9f1083fe2a8"),
						Name = "Hotel Nights",
                        ConsumptionType = (ConsumptionType)16,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("70569d47-a4fa-4b8e-93ba-0543418cdce8")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("5e65ee17-0094-47be-8cda-9bcfc711cc7e")),
                            context.ActivityGroups.Find(new Guid("c503d3c2-2762-499a-a3a6-b0f0ff23f593")),
                            context.ActivityGroups.Find(new Guid("abf85078-4cd3-4f44-8bc0-42ca7b8f5a7f")),
                            context.ActivityGroups.Find(new Guid("7e1f78a1-4a4c-462f-b09e-bdf92d16d8cf")),
                            context.ActivityGroups.Find(new Guid("495a2d86-8bf0-4721-8095-2512b4289352")),
                            context.ActivityGroups.Find(new Guid("fa6477f3-f0fa-42dd-b7a6-ed7b1ba6c637")),
                            context.ActivityGroups.Find(new Guid("36cd5aa7-d264-4b01-87b1-4b859945322f")),
                            context.ActivityGroups.Find(new Guid("fe35756b-60ac-4907-b8a9-4f51e3af953e")),
                            context.ActivityGroups.Find(new Guid("844969cb-11e5-43d3-8069-edddc28df524")),
                            context.ActivityGroups.Find(new Guid("e0e780e2-b489-40a0-a70a-df8ac7a484d3")),
                            context.ActivityGroups.Find(new Guid("d92c54c4-0dbd-4f33-aef3-f6b1bfb3124c")),
                            context.ActivityGroups.Find(new Guid("f5716491-59c8-40fd-8bc3-4eda82b8e8ff")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.AirTravelRoute.AirTravelRouteCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("4451ffcf-4851-46be-88e9-a2930a82a312"),
						Name = "Air Travel (Route)",
                        ConsumptionType = (ConsumptionType)32,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("0109781a-989b-4c99-b9d6-03b3a4259485")),
                            context.Factors.Find(new Guid("9262c558-1d4d-4f65-9874-367d1b4c3085")),
                            context.Factors.Find(new Guid("6d3269d5-6339-4697-9f59-dcedcd8b3655")),
                            context.Factors.Find(new Guid("61730c1c-a6df-49da-837f-66c687d0669b")),
                            context.Factors.Find(new Guid("90309bb6-72f3-4967-8fa9-412569c5a78d")),
                            context.Factors.Find(new Guid("825e3edc-c89f-425c-b4f9-2cd109f6a8a0")),
                            context.Factors.Find(new Guid("1ab050b7-26fa-420c-9f69-9b0ffe419ed5")),
                            context.Factors.Find(new Guid("0d7d7de7-db31-489b-b40b-b6c282b56436")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("5e65ee17-0094-47be-8cda-9bcfc711cc7e")),
                            context.ActivityGroups.Find(new Guid("c503d3c2-2762-499a-a3a6-b0f0ff23f593")),
                            context.ActivityGroups.Find(new Guid("abf85078-4cd3-4f44-8bc0-42ca7b8f5a7f")),
                            context.ActivityGroups.Find(new Guid("7e1f78a1-4a4c-462f-b09e-bdf92d16d8cf")),
                            context.ActivityGroups.Find(new Guid("495a2d86-8bf0-4721-8095-2512b4289352")),
                            context.ActivityGroups.Find(new Guid("fa6477f3-f0fa-42dd-b7a6-ed7b1ba6c637")),
                            context.ActivityGroups.Find(new Guid("36cd5aa7-d264-4b01-87b1-4b859945322f")),
                            context.ActivityGroups.Find(new Guid("fe35756b-60ac-4907-b8a9-4f51e3af953e")),
                            context.ActivityGroups.Find(new Guid("844969cb-11e5-43d3-8069-edddc28df524")),
                            context.ActivityGroups.Find(new Guid("e0e780e2-b489-40a0-a70a-df8ac7a484d3")),
                            context.ActivityGroups.Find(new Guid("d92c54c4-0dbd-4f33-aef3-f6b1bfb3124c")),
                            context.ActivityGroups.Find(new Guid("f5716491-59c8-40fd-8bc3-4eda82b8e8ff")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.AirTravel.AirTravelCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("89a1c672-f4fb-4753-974d-26dd86deb0db"),
						Name = "Air Travel",
                        ConsumptionType = (ConsumptionType)32,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("0109781a-989b-4c99-b9d6-03b3a4259485")),
                            context.Factors.Find(new Guid("9262c558-1d4d-4f65-9874-367d1b4c3085")),
                            context.Factors.Find(new Guid("6d3269d5-6339-4697-9f59-dcedcd8b3655")),
                            context.Factors.Find(new Guid("61730c1c-a6df-49da-837f-66c687d0669b")),
                            context.Factors.Find(new Guid("90309bb6-72f3-4967-8fa9-412569c5a78d")),
                            context.Factors.Find(new Guid("825e3edc-c89f-425c-b4f9-2cd109f6a8a0")),
                            context.Factors.Find(new Guid("1ab050b7-26fa-420c-9f69-9b0ffe419ed5")),
                            context.Factors.Find(new Guid("0d7d7de7-db31-489b-b40b-b6c282b56436")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("e83a868e-d8d0-40bb-8525-2e929939c55b")),
                            context.ActivityGroups.Find(new Guid("bb532ea7-2719-4daa-94b4-47d3acfa0f3a")),
                            context.ActivityGroups.Find(new Guid("23cbba5d-c485-4feb-8a72-f1208885d2cb")),
                            context.ActivityGroups.Find(new Guid("2d590daa-bbf9-49d4-b608-4d7d67db80d8")),
                            context.ActivityGroups.Find(new Guid("88908535-ae8d-4f08-95fa-b5e3eacdcfce")),
                            context.ActivityGroups.Find(new Guid("ec7a4b01-78b8-4ce5-b24a-d24afe8f9dc3")),
                            context.ActivityGroups.Find(new Guid("6331c5f4-1d3e-447f-aa9b-5c6cbc73537b")),
                            context.ActivityGroups.Find(new Guid("db2125e4-ef77-43d4-90a3-daab937bbd29")),
                            context.ActivityGroups.Find(new Guid("9a1bee6c-91ea-41c6-9c3c-db77023f15c2")),
                            context.ActivityGroups.Find(new Guid("82c020cb-afaa-4673-81e2-e036abc75c4e")),
                            context.ActivityGroups.Find(new Guid("c30c34ea-e8ce-41be-9652-f567aae80be9")),
                            context.ActivityGroups.Find(new Guid("59d006f9-fd59-4be0-b62d-7d82f22723a1")),
                            context.ActivityGroups.Find(new Guid("0ba90e64-71c2-4c65-8c37-dc9a7b92f07a")),
                            context.ActivityGroups.Find(new Guid("5c87d6f8-8216-49ec-9898-a26e9ad38d03")),
                            context.ActivityGroups.Find(new Guid("26806405-bcc7-46c5-afb9-ac8b77ff3ec2")),
                            context.ActivityGroups.Find(new Guid("27ef89bc-644c-490b-a7ef-39ffbf7ed8a6")),
                            context.ActivityGroups.Find(new Guid("7bbc5ea6-deac-428c-be48-81b1125031ba")),
                            context.ActivityGroups.Find(new Guid("21c92a9e-5e0d-4fdd-804f-c0aa1617bdcf")),
                            context.ActivityGroups.Find(new Guid("794604e6-9a56-4e85-91a6-66ceb85c975b")),
                            context.ActivityGroups.Find(new Guid("83a5f725-9b26-4c13-9dbe-27426ca25304")),
                            context.ActivityGroups.Find(new Guid("cfeddf1e-91c2-4f4d-9895-5f0c12b8ba8e")),
                            context.ActivityGroups.Find(new Guid("dad06b6e-2e8d-41f1-b90c-dac78689099a")),
                            context.ActivityGroups.Find(new Guid("a5d9ff6a-57f8-4baa-8156-96479ae11b96")),
                            context.ActivityGroups.Find(new Guid("ed5d320a-c7a9-460d-a954-0eaea5836031")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.CarHire.CarHireCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("d60848df-7852-4bff-8c80-ecdb21ae328b"),
						Name = "Car Hire",
                        ConsumptionType = (ConsumptionType)8,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("fe6e13b0-575d-4822-ac04-f100cf01a6ee")),
                            context.Factors.Find(new Guid("692ec00a-315b-49bb-8aec-4d86422c8ccb")),
                            context.Factors.Find(new Guid("3afd8119-5090-4863-a474-6f4cec6a2821")),
                            context.Factors.Find(new Guid("36df1d51-4b1f-4552-ac83-6c6cec4f6f71")),
                            context.Factors.Find(new Guid("3195f930-cdf5-482a-b393-7589236467c1")),
                            context.Factors.Find(new Guid("e8bf433f-214d-4d89-8c02-a2794a6f1938")),
                            context.Factors.Find(new Guid("269f3ef3-e74d-4dcc-b5a7-3e5e3f05a189")),
                            context.Factors.Find(new Guid("3eb227d9-b369-47eb-9a0a-bdde5e44a2f8")),
                            context.Factors.Find(new Guid("2f1cfef5-4801-4afd-80bf-edc1642fb00b")),
                            context.Factors.Find(new Guid("c3e1ceef-51e8-435f-8ab7-77d0b759bd30")),
                            context.Factors.Find(new Guid("585dce22-93b4-4da4-aa8b-43c5a26ef4b1")),
                            context.Factors.Find(new Guid("5f7d5b47-79a0-4838-a1f5-1b7e353eb7e0")),
                            context.Factors.Find(new Guid("6fdfe1fc-8d82-47c7-970c-fcd7bc174e7c")),
                            context.Factors.Find(new Guid("2944b143-a0a1-4755-9d62-b7e6b08fc149")),
                            context.Factors.Find(new Guid("84c39f06-3c18-406a-9fdb-f143d4333e3b")),
                            context.Factors.Find(new Guid("06909133-805e-4271-9fc2-6be805588a28")),
                            context.Factors.Find(new Guid("5a15585c-f779-433b-853b-e037c1c54b80")),
                            context.Factors.Find(new Guid("4c639165-9531-4a31-b32e-fd8e2b43915e")),
                            context.Factors.Find(new Guid("3205ae00-929e-445f-a31c-7a48f95ccc55")),
                            context.Factors.Find(new Guid("7625c31c-b6e9-46d6-9fd8-fc2a39d14533")),
                            context.Factors.Find(new Guid("99bbef2a-f249-4f9a-8f1a-729bc403a0af")),
                            context.Factors.Find(new Guid("f899d184-db66-403d-b2d1-2164c23caf0c")),
                            context.Factors.Find(new Guid("72e5b386-3778-4200-b36a-d3d08c70a365")),
                            context.Factors.Find(new Guid("41380c49-96cc-4a8f-9d57-ec983f67d77e")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("a9ef1de7-047e-48e8-8418-5e8160ae9d88")),
                            context.ActivityGroups.Find(new Guid("a61123b5-3290-473b-a71c-cf19fc2b3753")),
                            context.ActivityGroups.Find(new Guid("fbbf79ec-0a14-4df4-b799-925c31ad57c0")),
                            context.ActivityGroups.Find(new Guid("232e1c65-2c70-42a0-89ee-479182902f41")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.Commuting.CommutingCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("fb1c2b6d-39b9-4153-995b-1cdd6823bd71"),
						Name = "Commuting",
                        ConsumptionType = (ConsumptionType)256,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("41d7e99a-4e44-4d5a-9a21-3cd3bc56bc78")),
                            context.Factors.Find(new Guid("a6e0745a-b821-48c8-ab4f-649748bf33ac")),
                            context.Factors.Find(new Guid("06b322c3-71c5-4bfd-a08c-6fcf5b6adb0f")),
                            context.Factors.Find(new Guid("e8a0b935-64a3-4fcb-9a7e-a9246d2be7f0")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("28981ece-b602-4082-9920-e8eb5afdc48c")),
                            context.ActivityGroups.Find(new Guid("a24dfcee-7638-4f10-979b-7439efc406b4")),
                            context.ActivityGroups.Find(new Guid("a303fb75-f015-4147-b347-e57cf2c130fb")),
                            context.ActivityGroups.Find(new Guid("684d8547-8667-4a0e-a70f-39677975a5e9")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.CourierRoute.CourierRouteCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("183d2234-8a1d-4599-8ac7-dd5fcc5af819"),
						Name = "Courier (Route)",
                        ConsumptionType = (ConsumptionType)128,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("18c261b5-54d0-4f8b-bbf5-65a2395a7b46")),
                            context.Factors.Find(new Guid("411f45fc-3cbc-4126-ad50-679fe05686fb")),
                            context.Factors.Find(new Guid("60c6495c-34a1-480f-ae43-d30cf454dbbd")),
                            context.Factors.Find(new Guid("166b54ea-627e-41b2-9c95-e0b9f9c85ae2")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("28981ece-b602-4082-9920-e8eb5afdc48c")),
                            context.ActivityGroups.Find(new Guid("a24dfcee-7638-4f10-979b-7439efc406b4")),
                            context.ActivityGroups.Find(new Guid("a303fb75-f015-4147-b347-e57cf2c130fb")),
                            context.ActivityGroups.Find(new Guid("684d8547-8667-4a0e-a70f-39677975a5e9")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.Courier.CourierCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("44a2cc3f-d365-4e78-a3bc-eea835f59afb"),
						Name = "Courier",
                        ConsumptionType = (ConsumptionType)128,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("18c261b5-54d0-4f8b-bbf5-65a2395a7b46")),
                            context.Factors.Find(new Guid("411f45fc-3cbc-4126-ad50-679fe05686fb")),
                            context.Factors.Find(new Guid("60c6495c-34a1-480f-ae43-d30cf454dbbd")),
                            context.Factors.Find(new Guid("166b54ea-627e-41b2-9c95-e0b9f9c85ae2")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("8f173eb8-e5b2-4d57-b6c8-aa9353c0a08a")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.Electricity.ElectricityCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("c1809f62-369d-413f-a643-2489fdb8d3a3"),
						Name = "Electricity",
                        ConsumptionType = (ConsumptionType)2,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("c4d57fde-2538-41e3-baa1-7c3a479ab81e")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("5159658c-4194-4f33-84ca-9f9466122d03")),
                            context.ActivityGroups.Find(new Guid("f2bc7a11-7c5c-47c3-b9b9-6745a0465704")),
                            context.ActivityGroups.Find(new Guid("04b3941a-0096-4c7f-bfcb-34669c671ef9")),
                            context.ActivityGroups.Find(new Guid("a56ec01c-ec31-4a01-92f7-0e149583ebab")),
                            context.ActivityGroups.Find(new Guid("ba7a7843-32d0-438d-8d34-5848b0d175eb")),
                            context.ActivityGroups.Find(new Guid("dfcd7ce0-1e73-4eda-8231-ba579e50540e")),
                            context.ActivityGroups.Find(new Guid("8caea66f-d10c-4739-89d9-1c176d6aabd9")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.Fuel.FuelCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("d70ed5bc-6ad9-45e7-9498-27c0f131b449"),
						Name = "Fuel",
                        ConsumptionType = (ConsumptionType)256,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("92ab41d3-a874-415a-8b15-9367431735ff")),
                            context.Factors.Find(new Guid("f55de783-de44-499a-ac9e-f083ec70e1a4")),
                            context.Factors.Find(new Guid("c0715292-0f24-41d1-9511-b2bf050ad605")),
                            context.Factors.Find(new Guid("e516798f-1009-443b-b747-50ef93dca163")),
                            context.Factors.Find(new Guid("e30bdfe4-00e5-4432-ba3c-47af6555697a")),
                            context.Factors.Find(new Guid("60ca828a-ccba-461b-8c3f-39780533b3eb")),
                            context.Factors.Find(new Guid("dff978ea-ed38-47fd-ab45-06de179840e3")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("1692c162-9917-4575-b18e-62c015d9dc6c")),
                            context.ActivityGroups.Find(new Guid("9b7cc2a9-5efe-4283-9fc6-6ea276f53064")),
                            context.ActivityGroups.Find(new Guid("7b2bf3b7-4722-4ff6-a5d6-01081e4ad245")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.Paper.PaperCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("b3e38e60-4e6b-4c5b-a422-3500fc8dabc4"),
						Name = "Paper",
                        ConsumptionType = (ConsumptionType)4,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("a7a80d93-da3b-48a5-868c-996846496161")),
                            context.Factors.Find(new Guid("072a30f6-b9f8-4a20-bc73-48b0fa0590d9")),
                            context.Factors.Find(new Guid("fea803dc-0058-4f03-91df-1749112745d2")),
                            context.Factors.Find(new Guid("e9c65569-13f1-49d5-9a69-8599c69cb14b")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("304881c3-de27-4eec-97b8-11b98f025bd5")),
                            context.ActivityGroups.Find(new Guid("6514f78b-677c-477b-a142-ae002b8df989")),
                            context.ActivityGroups.Find(new Guid("2bdfcffb-3f32-4c46-a4b9-426cc8d43991")),
                            context.ActivityGroups.Find(new Guid("0e14558e-aae4-4ec8-90f2-443a324f9c41")),
                            context.ActivityGroups.Find(new Guid("144e732c-a398-4b55-834e-e94aff2bb8e6")),
                            context.ActivityGroups.Find(new Guid("c29e7a56-7ae0-4a6f-8630-22b2d959ec10")),
                            context.ActivityGroups.Find(new Guid("f6989dcd-bed9-40fe-870c-a6f9a6ef1d81")),
                            context.ActivityGroups.Find(new Guid("f556504d-3367-4b45-ada3-58d1a1c1e51d")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.Refrigerant.RefrigerantCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("dc5b2b93-9267-4119-b0ec-5c7407ebd230"),
						Name = "Refrigerant",
                        ConsumptionType = (ConsumptionType)256,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("585e0250-1396-464f-8476-12877913b6c9")),
                            context.Factors.Find(new Guid("46ee37fb-eea1-4a33-8c45-ef7ca8fd526f")),
                            context.Factors.Find(new Guid("0204f144-b401-4983-bec8-2b71b77d39fd")),
                            context.Factors.Find(new Guid("3d323103-f97f-4b8a-be3a-e174d59e4f01")),
                            context.Factors.Find(new Guid("66160f37-ae8f-4dfb-9717-cf8640be53c1")),
                            context.Factors.Find(new Guid("3e5d69be-182c-41a0-811b-e5c1464cb903")),
                            context.Factors.Find(new Guid("054af8e5-130f-47e1-be8b-6d9dfc0fedd3")),
                            context.Factors.Find(new Guid("83030c6b-36f4-4fa5-aa89-7cd551d07dfa")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("5585fa9b-ec8c-4271-8e9c-6d3d24a7ebbe")),
                            context.ActivityGroups.Find(new Guid("e942c0a6-7324-4ea4-8b05-376d93bb5cd3")),
                            context.ActivityGroups.Find(new Guid("49c489e0-a7dc-460b-9451-f41d05ec4baa")),
                            context.ActivityGroups.Find(new Guid("e05679ac-5ede-4dad-b488-9c436a304336")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.Fleet.FleetCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("3c13f9c9-e7e6-4aed-8b0d-55ff484f8d8f"),
						Name = "Vehicle",
                        ConsumptionType = (ConsumptionType)256,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("203de165-1678-491c-8c32-9cb19781ff42")),
                            context.Factors.Find(new Guid("48d0167d-53fa-4b44-83d5-c5a571409ea5")),
                            context.Factors.Find(new Guid("76be78a6-55b6-461b-a712-d8deccc0befc")),
                            context.Factors.Find(new Guid("281bd4da-c051-4dce-bc30-062d36e8a989")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("0fd75862-7331-43e6-a018-49fbe1a151ec")),
                            context.ActivityGroups.Find(new Guid("31f7b3ad-486f-4630-be73-ae13bc7d69a2")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.Waste.WasteCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("fb995404-7f1c-4041-97d7-f5bb8ec5b7bd"),
						Name = "Waste",
                        ConsumptionType = (ConsumptionType)64,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("aa354f3d-a91f-46f4-84fc-991048d19d23")),
                            context.Factors.Find(new Guid("3a2aca96-9085-4099-9130-f027530387e4")),
						}
					},                new Calculation
                {
                    ActivityGroups = new[]
						{
                            context.ActivityGroups.Find(new Guid("e07eb14d-593a-4e32-8eb9-b0411f278ea3")),
						},
						AssemblyQualifiedName = "CarbonKnown.Calculation.Water.WaterCalculation, CarbonKnown.Calculation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
						Id = new Guid("2b8cf591-19ab-4251-b083-2db440972f23"),
						Name = "Water",
                        ConsumptionType = (ConsumptionType)1,
						Factors = new[]
						{
                            context.Factors.Find(new Guid("9b06348f-2e2e-4b91-9a2b-3a21d852d55b")),
						}
					});
		}
	}
}
