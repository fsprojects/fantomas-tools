FROM mcr.microsoft.com/dotnet/sdk:5.0

# Install .NET Core 3.1 SDK
RUN dotnet_sdk_version=3.1.404 \
    && curl -SL --output dotnet.tar.gz https://dotnetcli.azureedge.net/dotnet/Sdk/$dotnet_sdk_version/dotnet-sdk-$dotnet_sdk_version-linux-x64.tar.gz \
    && dotnet_sha512='94d8eca3b4e2e6c36135794330ab196c621aee8392c2545a19a991222e804027f300d8efd152e9e4893c4c610d6be8eef195e30e6f6675285755df1ea49d3605' \
    && echo "$dotnet_sha512 dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -ozxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz

# NodeJS 14.X
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash
RUN apt-get install -y nodejs

# Yarn
RUN npm install -g yarn

# Install Azure Function Core Tools
RUN npm i -g azure-functions-core-tools@3 --unsafe-perm true
