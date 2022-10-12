open Fable.Remoting.Giraffe
open Fable.Remoting.Server
open Saturn
open Shared

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue Api.dojoApi
    |> Remoting.buildHttpHandler

let app = application {
    // Listen on all interfaces: ipv4 **and** ipv6
    // See these issues for more details:
    // [devServerProxy not working (ECONNREFUSED)](https://github.com/SAFE-Stack/SAFE-template/issues/518)
    // [Listen on all interfaces including ipv6](https://github.com/SAFE-Stack/SAFE-template/pull/490)
    // [Upgrade webpack and npm deps](https://github.com/SAFE-Stack/SAFE-template/pull/485)
    url "http://*:8085"
    // Only listens on ipv4 interfaces
    // url "http://0.0.0.0:8085"
    use_router webApp
    use_static "public"
    use_gzip
}

run app