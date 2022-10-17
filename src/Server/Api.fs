module Api

open DataAccess
open FSharp.Data.UnitSystems.SI.UnitNames
open Shared

let london =
    { Latitude = 51.5074
      Longitude = 0.1278 }

let getDistanceFromLondon postcode = async {
    if not (Validation.isValidPostcode postcode) then failwith "Invalid postcode"

    let! location = getLocation postcode
    let distanceToLondon = getDistanceBetweenPositions location.LatLong london
    return { Postcode = postcode; Location = location; DistanceToLondon = (distanceToLondon / 1000.<meter>) }
}

let getCrimeReport postcode = async {
    if not (Validation.isValidPostcode postcode) then failwith "Invalid postcode"

    let! location = getLocation postcode
    let! reports = getCrimesNearPosition location.LatLong
    let crimes =
        reports
        |> Array.countBy(fun r -> r.Category)
        |> Array.sortByDescending snd
        |> Array.map(fun (k, c) -> { Crime = k; Incidents = c })
    return crimes
}

let private asWeatherResponse (weather: Weather.OpenMeteoCurrentWeather.CurrentWeather) =
    { WeatherType = weather.Weathercode |> WeatherType.FromCode
      Temperature = float weather.Temperature }

let getWeather postcode = async {
    // Lookup the location information for the specified postal code
    let! location = getLocation postcode
    let! weatherResponse = getWeatherForPosition location.LatLong
    return asWeatherResponse weatherResponse
}

let dojoApi =
    {
      GetDistance = getDistanceFromLondon

      GetCrimes = getCrimeReport

      GetWeather = getWeather
    }