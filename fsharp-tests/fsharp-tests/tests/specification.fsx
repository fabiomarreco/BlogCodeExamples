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
    with 
        static member (&&&) (a:ProductSpecification, b:ProductSpecification) = And (a, b)
        static member (|||) (a:ProductSpecification, b:ProductSpecification) = Or (a, b)


(MatchesCategory "category1") &&& ((NameContains "Name") ||| (NameContains "name2"))



let spec = (MatchesCategory "category1") ||> And (NameContains "name") |> ff Or (NameContains "name2")



And (NameContains "Name")

//--------------------------------------

#I @"../../packages/build/SQLProvider/lib/net451"
#r "FSharp.Data.SqlProvider.dll"
open FSharp.Data.Sql

let [<Literal>] resolutionPath = __SOURCE_DIRECTORY__ + @"/../../packages/build/System.Data.SQLite.Core/lib/net451"
let [<Literal>] connectionString = "Data Source=" + __SOURCE_DIRECTORY__+ @"/../../files/sample.db;Version=3"


type sql = SqlDataProvider< 
              ConnectionString = connectionString,
              DatabaseVendor = Common.DatabaseProviderTypes.SQLITE,
              ResolutionPath = resolutionPath,
              IndividualsAmount = 1000,
              UseOptionTypes = true >


let ctx = sql.GetDataContext();





let ff j n = fun p -> j (p, n)
let (&>) prev next = And (prev, next)

let (!!) prev oper =




let q1 = 
    query { 
        for product in ctx.Main.Product do
        where(product.Name = Some "IPad")
        select (product) }
    |> Seq.toList

