#r @"./packages/SQLProvider/lib/net451/FSharp.Data.SqlProvider.dll"
//#r @"./packages/System.Data.SqlClient/lib/netstandard2.0/System.Data.SqlClient.dll"
//#I @"./packages/SQLProvider/lib/netstandard2.0/"
//#r @"./packages/NETStandard.Library.NETFramework/build/net461/lib/netstandard.dll"
#r @"./packages/Microsoft.Data.Sqlite.Core/lib/netstandard2.0/Microsoft.Data.Sqlite.dll"
//#r @"./packages/SQLProvider/lib/netstandard2.0/FSharp.Data.SqlProvider.dll"


open FSharp.Data.Sql

type Category = string
type Money = double
type PriceRange = { LowerBound : Money; UpperBound : Money }

type ProductSpecification = 
    | MatchesCategory of Category
    | PriceInRange of PriceRange
    | NameContains of string
    | And of (ProductSpecification * ProductSpecification)
    | Or of (ProductSpecification * ProductSpecification)
    | Not of ProductSpecification


let ff j n = fun p -> j (p, n)
let (&>) prev next = And (prev, next)



let spec = (MatchesCategory "category1") |> ff And (NameContains "name") |> ff Or (NameContains "name2")



let [<Literal>] resolutionPath = __SOURCE_DIRECTORY__ + @"\files\"
let [<Literal>] connectionString = "Data Source=" + __SOURCE_DIRECTORY__ + @"\files\testing.db;Version=3" 

type sql = SqlDataProvider< 
              ConnectionString = connectionString,
              DatabaseVendor = Common.DatabaseProviderTypes.SQLITE,
              ResolutionPath = resolutionPath,
              IndividualsAmount = 1000,
              UseOptionTypes = true >

