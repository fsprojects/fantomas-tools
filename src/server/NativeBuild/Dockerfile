# Where the native lambdas are compiled: Amazon Linux 2023, which Lambda's provided.al2023 runs.
# Native AOT links against the glibc of the machine it compiles on, and the executable needs that
# glibc or a newer one, so one compiled here starts on Lambda whatever machine runs the build.
# The `publish native lambda` steps of build.fsx build this image and publish in it.
#
# Pinned by digest, which pins the packages too: Amazon Linux 2023 locks its package repositories to
# the release the image is. Dependabot moves the digest.
FROM public.ecr.aws/amazonlinux/amazonlinux:2023@sha256:c07ab0773eddf9449c0994a87d8148300db98bdf8d8dad672a9dc26321c104fc

RUN dnf install -y --setopt=install_weak_deps=False clang zlib-devel libicu tar gzip findutils \
    && dnf clean all

# The SDK global.json asks for, and its SHA-512, both looked up by build.fsx.
ARG SDK_URL
ARG SDK_SHA512
RUN curl -sSLf -o /tmp/dotnet-sdk.tar.gz "$SDK_URL" \
    && echo "$SDK_SHA512  /tmp/dotnet-sdk.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -xzf /tmp/dotnet-sdk.tar.gz -C /usr/share/dotnet \
    && rm /tmp/dotnet-sdk.tar.gz

ENV PATH=/usr/share/dotnet:$PATH \
    DOTNET_ROOT=/usr/share/dotnet \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1
