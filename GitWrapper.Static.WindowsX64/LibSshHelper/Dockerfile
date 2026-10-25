FROM debian:bookworm-slim AS build

RUN apt-get update \
  && apt-get install -y --no-install-recommends \
    gcc-mingw-w64-x86-64 \
  && rm -rf /var/lib/apt/lists/*

WORKDIR /src

COPY main.c .

RUN mkdir -p /out \
  && x86_64-w64-mingw32-gcc \
    -std=c11 \
    -Wall \
    -Wextra \
    -O2 \
    -static \
    -s \
    main.c \
    -o /out/ssh-passphrase.exe

FROM scratch
COPY --from=build /out/ssh-passphrase.exe /ssh-passphrase.exe

# Use one of the two commands to create:
# docker buildx build --platform linux/amd64 --output type=local,dest=./out .
# mkdir -p ./out && docker buildx build --platform linux/amd64 --output type=tar,dest=- . | tar -xf - -C ./out/
