﻿open System
open System.IO

open Data

[<EntryPoint>]
let main argv = 
    let path        = "SFBay.csv"
    let path_json   = "InfoVisProject.App/data/data.js"
    let lines       = File.ReadAllLines(path)|> Array.map (fun line -> line.Split(';') )
                                        
   
    let header  = Array.head lines
    let header  = [|header.[0]; header.[1]; header.[4]; header.[17]; header.[18]|]
    let data    = Array.tail lines

    let timeStamp           = data |> Array.map(fun line -> line.[0].Substring(0, 4))
    let stationNumber       = data |> Array.map(fun line -> line.[1])
    let discreteChlorophyll = data |> Array.map(fun line -> line.[4])   |> Array.map (fun sample -> sample.Replace('.',','))
    let salinity            = data |> Array.map(fun line -> line.[17])  |> Array.map (fun sample -> sample.Replace('.',','))
    let temperature         = data |> Array.map(fun line -> line.[18])  |> Array.map (fun sample -> sample.Replace('.',','))
    

    // Reduced Dimensions
    let data =  timeStamp |> Array.mapi (fun i t -> [|t ; stationNumber.[i];discreteChlorophyll.[i];salinity.[i];temperature.[i]|])


    


    let nonStandardStations = [|"662";"659";"655";"654";"653";"652";"651";"650";"411";"407";"405";"12.5";"19";"28.5"; "5"; "6"; "7"|]


    
    
    
    let samples = data |> Array.map (fun sample -> 
                                            let c = 
                                                match sample.[2] with 
                                                | ""    -> None
                                                | _     -> Some (System.Double.Parse(sample.[2], Globalization.NumberStyles.AllowDecimalPoint))
                                            let s = 
                                                match sample.[3] with 
                                                | ""    -> None
                                                | _     -> Some (System.Double.Parse(sample.[3], Globalization.NumberStyles.AllowDecimalPoint))
                                            let t = 
                                                match sample.[4] with 
                                                | ""    -> None
                                                | _     -> Some (System.Double.Parse(sample.[4], Globalization.NumberStyles.AllowDecimalPoint))
                                            
                                            {
                                            year                = sample.[0]
                                            station             = sample.[1]
                                            discreteChlorophyll = c
                                            salinity            = s
                                            temperature         = t
                                            }
                                        )
    
                            // filter non standard stations
    let samplesAveraged = samples |> Array.filter (fun sample -> nonStandardStations |> Array.contains sample.station |> not)
                                  |> Array.groupBy(fun sample -> sample.year + sample.station)
                                  |> Array.map  (   fun (_,samples) ->
                                                    let year        = samples.[0].year
                                                    let station     = samples.[0].station
                                                    let salinity    = samples   |> Array.choose (fun sample -> sample.salinity)                        
                                                    let temperature = samples   |> Array.choose (fun sample -> sample.temperature)                      
                                                    let chlorophyll = samples   |> Array.choose (fun sample -> sample.discreteChlorophyll)
                                                    
                                                    let avgSalinity = 
                                                        match salinity with
                                                        | [||]  -> None
                                                        | _     -> salinity |> Array.average |> Some
                                                    
                                                    let avgTemperature = 
                                                        match temperature with
                                                        | [||]  -> None
                                                        | _     -> temperature |> Array.average |> Some

                                                    let avgChlorophyll = 
                                                        match chlorophyll with
                                                        | [||]  -> None
                                                        | _     -> chlorophyll |> Array.average |> Some
                                                    
                                                    {
                                                        station             = station
                                                        year                = year
                                                        salinity            = avgSalinity
                                                        temperature         = avgTemperature
                                                        discreteChlorophyll = avgChlorophyll
                                                    }
                                                )
                                                    
    
    //let minYear = ((samplesAveraged |> Array.sortBy(fun sample -> sample.year + sample.station)).[0]            ).year
    //let maxYear = ((samplesAveraged |> Array.sortBy(fun sample -> sample.year + sample.station)) |> Array.last  ).year
    
    //let minYear = System.Int32.Parse(minYear)
    //let maxYear = System.Int32.Parse(maxYear) 
    

    //for year in minYear..maxYear do
    //    let year = sprintf "%i" year
       
    //    let numStations = samplesAveraged |> Array.filter (fun sample -> sample.year = year)
    //                                      |> Array.map (fun sample -> sample.year)
    //    printfn "%s: stations: %i" year (numStations |> Array.length)
    

    

    //let maxSalinity = samplesAveraged |> Array.maxBy(fun sample -> sample.salinity)

    //let tempDiff = abs(maxTemp.temperature - minTemp.temperature)

    // Build averaged samples
    Data.WriteToJson path_json samplesAveraged
    printfn "Press any key to close..."
    Console.ReadKey() |> ignore
    0