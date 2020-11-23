FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# NodeJS 12.X
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash
RUN apt-get install -y nodejs

# Yarn
RUN npm install -g yarn

# Install Azure Function Core Tools
RUN npm i -g azure-functions-core-tools@3 --unsafe-perm true
