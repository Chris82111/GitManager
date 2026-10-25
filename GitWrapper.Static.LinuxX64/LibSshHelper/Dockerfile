FROM alpine:latest AS build

RUN apk add --no-cache \
  musl-dev \
  gcc

WORKDIR /src

COPY main.c .

RUN mkdir -p /out \
  && gcc \
    -std=c11 \
    -Wall \
    -Wextra \
    -Os \
    -static \
    -s \
    main.c \
    -o /out/ssh-passphrase

FROM scratch
COPY --from=build /out/ssh-passphrase /ssh-passphrase

# Build with:
# docker buildx build --platform linux/amd64 --output type=local,dest=./out .
