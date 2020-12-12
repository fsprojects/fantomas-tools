FROM mcr.microsoft.com/dotnet/sdk:5.0

# Install .NET Core 3.1 SDK
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 3.1 --install-dir /usr/share/dotnet

# NodeJS 14.X
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash
RUN apt-get install -y nodejs

# Yarn
RUN npm install -g yarn

# Install Azure Function Core Tools
RUN npm i -g azure-functions-core-tools@3 --unsafe-perm true
