#!/bin/bash
# Build an Actian Ingres Client Docker image.
# The image is loaded into the local Docker daemon and also saved to a tar file.
#
# Usage:
#   ./build.sh odbc2.3.9    – libodbc2 2.3.9  (Ubuntu default)
#   ./build.sh odbc2.3.12   – unixODBC 2.3.12 (built from source)
#   ./build.sh all           – build both images

set -e

# ── ESD download URL ─────────────────────────────────────────
# Supply the pre-signed token URL from the Actian ESD portal before running
# this script. No username or password is needed – the token embedded in the
# URL is sufficient for authorisation.
#
#   export ESD_DOWNLOAD_URL="https://esdftp.actian.com/~<token>/actian-client-1.4.0-20-free-linux-ingbuild-x86_64.tgz"
if [ -z "${ESD_DOWNLOAD_URL:-}" ]; then
    echo "ERROR: ESD_DOWNLOAD_URL must be set before building."
    echo ""
    echo "  Log in at https://esd.actian.com/, navigate to your product,"
    echo "  right-click the download button and copy the link address."
    echo ""
    echo "Example:"
    echo "  export ESD_DOWNLOAD_URL=\"https://esdftp.actian.com/~oTnpdA/actian-client-1.4.0-20-free-linux-ingbuild-x86_64.tgz\""
    echo "  $0 odbc2.3.9"
    exit 1
fi

VERSION="${1:-}"

build_image() {
    local version="$1"

    case "$version" in
        odbc2.3.9)
            IMAGE_NAME="actian-ingres-client-env-odbc2.3.9"
            DOCKERFILE="Dockerfile_odbc2.3.9"
            ;;
        odbc2.3.12)
            IMAGE_NAME="actian-ingres-client-env-odbc2.3.12"
            DOCKERFILE="Dockerfile_odbc2.3.12"
            ;;
        *)
            echo "ERROR: unknown version '${version}'"
            echo "       Valid values: odbc2.3.9  odbc2.3.12"
            exit 1
            ;;
    esac

    IMAGE_TAG="latest"
    OUTPUT_TAR="${IMAGE_NAME}.tar"

    echo "============================================================"
    echo "Version    : ${version}"
    echo "Image      : ${IMAGE_NAME}:${IMAGE_TAG}"
    echo "Dockerfile : ${DOCKERFILE}"
    echo "Output tar : ${OUTPUT_TAR}"
    echo "============================================================"
    echo ""

    if [ ! -f "${DOCKERFILE}" ]; then
        echo "ERROR: ${DOCKERFILE} not found in current directory."
        exit 1
    fi

    echo "Building image..."
    docker build \
        --build-arg "ESD_DOWNLOAD_URL=${ESD_DOWNLOAD_URL}" \
        -t "${IMAGE_NAME}:${IMAGE_TAG}" \
        -f "${DOCKERFILE}" .

    echo ""
    echo "Saving image to ${OUTPUT_TAR}..."
    docker save "${IMAGE_NAME}:${IMAGE_TAG}" -o "${OUTPUT_TAR}"

    echo ""
    echo "Done."
    echo "  Image loaded : ${IMAGE_NAME}:${IMAGE_TAG}"
    echo "  Tarball saved: ${OUTPUT_TAR}"
    echo ""
    echo "To share with a customer, send: ${OUTPUT_TAR}"
    echo "Customer loads it with: docker load -i ${OUTPUT_TAR}"
    echo ""
}

case "$VERSION" in
    all)
        build_image odbc2.3.9
        build_image odbc2.3.12
        ;;
    odbc2.3.9|odbc2.3.12)
        build_image "$VERSION"
        ;;
    *)
        echo "Actian Ingres Client Docker image builder"
        echo ""
        echo "Usage:"
        echo "  $0 odbc2.3.9    – build image with libodbc2 2.3.9  (Ubuntu default)"
        echo "  $0 odbc2.3.12   – build image with unixODBC 2.3.12 (built from source)"
        echo "  $0 all           – build both images"
        echo ""
        exit 1
        ;;
esac
