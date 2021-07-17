FROM nojaf/fable-azure-functions:1.1 AS builder
WORKDIR /app
COPY .config /app/.config
COPY src /app/src
COPY build.fsx /app/build.fsx
COPY paket.dependencies /app/paket.dependencies
COPY paket.lock /app/paket.lock
RUN dotnet tool restore
RUN dotnet fake run build.fsx --list
RUN dotnet fake run build.fsx -t Fantomas-Git
RUN dotnet fake run build.fsx -t CI

FROM scratch as export-stage
COPY --from=builder /app/src/client/build /client
COPY --from=builder /app/artifacts /