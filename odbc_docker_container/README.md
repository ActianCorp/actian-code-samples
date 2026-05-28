# Actian Ingres Client – Test Environment

## Choose your version

This README covers **both** ODBC versions. Wherever you see `<VERSION>` or `<IMAGE_NAME>`, substitute the values from the row that matches your image:

| | unixODBC 2.3.9 | unixODBC 2.3.12 |
|---|---|---|
| **`<VERSION>`** | `odbc2.3.9` | `odbc2.3.12` |
| **`<IMAGE_NAME>`** | `actian-ingres-client-env-odbc2.3.9` | `actian-ingres-client-env-odbc2.3.12` |
| **Dockerfile** | `Dockerfile_odbc2.3.9` | `Dockerfile_odbc2.3.12` |
| **Extra build file** | *(none)* | *(none – downloaded automatically)* |
| **ODBC source** | Ubuntu 22.04 default package | Built from source inside the image |

---

## Environment summary

| Component | Version / Detail |
|---|---|
| OS | Ubuntu 22.04 LTS (Jammy Jellyfish) |
| Web server | Apache 2 (auto-starts on container launch) |
| PHP | Latest available for Ubuntu 22.04 |
| ODBC | See version table above |
| Actian Ingres Client | 1.4.0-20 (x86\_64) |

> **unixODBC 2.3.12 note:** Ubuntu 22.04 ships libodbc2 2.3.9 by default. The 2.3.12 image builds unixODBC from source and registers a dummy dpkg record so `php-odbc` satisfies its dependency without downgrading.

---

## Files in this package

| File | Required for | Purpose |
|---|---|---|
| `Dockerfile_odbc2.3.9` | 2.3.9 | Docker build definition |
| `Dockerfile_odbc2.3.12` | 2.3.12 | Docker build definition |
| `docker-compose.yml` | both | Convenience compose file (profile-separated) |
| `entrypoint.sh` | both | Container startup script (starts Apache and Actian client automatically) |
| `build.sh` | both | Builds and exports the image to a tar file |
| `odbc_php_update.php` | both | PHP test page deployed inside the container |
| `README.md` | both | This file |

---

## Prerequisites

### Windows
- [Docker Desktop for Windows](https://docs.docker.com/desktop/install/windows-install/) (requires WSL 2 or Hyper-V)
- PowerShell or Command Prompt

### Linux / macOS
- Docker Engine: `sudo apt install docker.io` (Ubuntu/Debian) or follow the [official guide](https://docs.docker.com/engine/install/)
- Docker Compose:
  - **v2 plugin** (recommended): `sudo apt install docker-compose-plugin` → use `docker compose`
  - **v1 standalone** (older systems): `sudo apt install docker-compose` → use `docker-compose`

Verify:
```bash
docker --version
docker compose version      # v2 plugin
docker-compose --version    # v1 standalone
```

---

## Step 1 – Obtain the image

Choose **Option A** (pre-built tar provided by Actian Support) or **Option B** (build from the source files yourself).

### Option A – Load a pre-built image tarball

```bash
docker load -i <IMAGE_NAME>.tar
```

**Windows (PowerShell):**
```powershell
docker load -i <IMAGE_NAME>.tar
```

Verify it loaded:
```bash
docker images | grep <IMAGE_NAME>
```

---

### Option B – Build the image from source

The Actian Ingres Client installer is downloaded directly from the [Actian ESD portal](https://esd.actian.com/) during the build using a pre-signed token URL. No username or password is required — the token embedded in the URL is sufficient for authorisation.

**1. Set the ESD download URL:**
```bash
export ESD_DOWNLOAD_URL="https://esdftp.actian.com/~oTnpdA/actian-client-1.4.0-20-free-linux-ingbuild-x86_64.tgz"
```

> **Note:** The `ESD_DOWNLOAD_URL` is the pre-signed download link from your ESD portal account. To obtain it:
> 1. Log in at [https://esd.actian.com/](https://esd.actian.com/) and navigate to the Actian Ingres Client package.
> 2. **Click the download button** to start the download in your browser.
> 3. Open the browser's **Downloads panel** (e.g. Ctrl+J in Chrome/Firefox).
> 4. Right-click the downloading/downloaded file and choose **"Copy download link"**.
> 5. Paste that link as `ESD_DOWNLOAD_URL` — it contains the temporary token (`~xxxxxx`) and is the only URL that works.
>
> *(Right-clicking the download button on the page and copying the link address will give a static URL without the token that does not work.)*
>
> The token is temporary — run the build script immediately after copying the link.

**2. Run the build script:**
```bash
chmod +x build.sh
./build.sh <VERSION>
```

The script:
1. Validates that `ESD_DOWNLOAD_URL` is set.
2. Passes it as a Docker build argument; the installer is downloaded inside the build and cleaned up – the URL is not stored in the final image.
3. Saves the finished image to **`<IMAGE_NAME>.tar`** for offline transfer.

After the build you can run the container immediately (skip `docker load`) or send the tar file to another machine.

> **Build time note (2.3.12 only):** The unixODBC 2.3.12 source compile adds a few minutes to the build. This is normal.

---

## Step 2 – Run the container

### Quick start (single command)

```bash
docker run -d \
  --name ingres-client-env \
  -p 8090:80 \
  <IMAGE_NAME>:latest
```

**Windows (PowerShell):**
```powershell
docker run -d `
  --name ingres-client-env `
  -p 8090:80 `
  <IMAGE_NAME>:latest
```

> If port **8090** is already in use, change the left-hand number (e.g. `-p 8091:80`) and use that port in all URLs below.

Apache is then accessible at: **http://localhost:8090**

---

### Using Docker Compose

A `docker-compose.yml` is included. It uses **profiles** to keep both ODBC versions separate.

```bash
# Load the image first (only needed once, or after a rebuild)
docker load -i <IMAGE_NAME>.tar

# Docker Compose v2 (plugin — preferred)
docker compose --profile <VERSION> up -d

# Docker Compose v1 (standalone — older systems)
docker-compose --profile <VERSION> up -d
```

To stop:
```bash
# v2
docker compose --profile <VERSION> down

# v1
docker-compose --profile <VERSION> down
```

> **Important:** Docker Compose does not pull or build the image automatically. Load the image into the local Docker daemon first (step above), otherwise you will get a `pull access denied` error.

---

## Step 3 – Access the container shell

By default, `docker exec` opens a shell as **root**:

```bash
docker exec -it ingres-client-env bash
```

To run Actian commands (`ingstart`, `netutil`, `iinamu`, etc.) you must be the **`actian`** user. Switch from the root shell:


```bash
su actian
```

> Steps 5 and 6 below assume you are running as the `actian` user.

---

## Step 4 – Verify the environment

Inside the container shell:

```bash
# Check Apache status
service apache2 status

# Check PHP version
php --version

# Check ODBC library version
dpkg -l libodbc2

# Verify Actian Ingres Client
export II_SYSTEM=/opt/Actian/Actian_Client
export PATH=$II_SYSTEM/ingres/bin:$II_SYSTEM/ingres/utility:$PATH
```

---

## Step 5 – Verify the Ingres Client network layer

The Actian Client network layer (`ingstart`) is started automatically when the container launches. To verify it is running, switch to the `actian` user (see Step 3) and run:

```bash
iinamu
```

Type `quit` to exit `iinamu`.

> If `iinamu` reports that the network layer is not running, start it manually:
> ```bash
> ingstart
> ```

---

## Step 6 – Create a vnode

A **vnode** maps a logical name to a remote Ingres server (hostname, port, and credentials).

Ensure you are the `actian` user (see Step 3), then run the connection manager:

```bash
netutil
```

Inside the `netutil` TUI:

1. Add a new Virtual Node.
2. Enter a vnode name (e.g. `myserver`).
3. Enter the remote server hostname or IP address.
4. Enter the listen address (typically `II` for a default Ingres installation).
5. Add **Login** credentials — enter the remote OS username and password.
6. Save and exit.

> If you are unsure of the connection parameters, contact the DBA of the remote Ingres server.

---

## Step 7 – Configure the DSN

The PHP test page connects using a DSN named **`demodb`**. Add it to the ODBC configuration file inside the container.

Edit the file:

```bash
vi /opt/Actian/Actian_Client/ingres/files/odbc.ini
```

Add the following entry (replace `<vnode-name>` with the vnode name from Step 6):

```ini
[demodb]
Server=<vnode-name>
```

Save and exit (`Esc`, then `:wq` in vi).

---

## Step 8 – Open the test page

Open a browser and go to:

```
http://localhost:8090/odbc_php_update.php
```

A successful connection prints the ODBC environment variables and executes a test UPDATE statement. If the connection fails, the page displays the ODBC error message.

**Note:** For ODBC version 2.3.12, once the above page is registered, a hanging DBMS session should be observed. You can check this on the remote Ingres server using `ipm`.

---

## Step 9 – Place your own web content (optional)

Apache serves files from `/var/www/html` inside the container.

**Copy a file in:**
```bash
docker cp myfile.php ingres-client-env:/var/www/html/myfile.php
```

**Or mount a local folder at run time:**
```bash
docker run -d \
  --name ingres-client-env \
  -p 8090:80 \
  -v /path/to/your/webroot:/var/www/html \
  <IMAGE_NAME>:latest
```

---

## Managing Apache

Apache starts automatically when the container starts. To restart it from inside the container shell:

```bash
sudo apache2ctl restart
# or
sudo service apache2 restart
```

---

## Useful commands

| Task | Command |
|---|---|
| Open a shell in the container | `docker exec -it ingres-client-env bash` |
| View Apache error log | `docker exec -it ingres-client-env tail -f /var/log/apache2/error.log` |
| View ODBC trace log | `docker exec -it ingres-client-env tail -f /var/www/html/logs/odbc_trace.log` |
| Start container | `docker start ingres-client-env` |
| Stop container | `docker stop ingres-client-env` |
| Remove container | `docker rm -f ingres-client-env` |
| Remove image | `docker rmi <IMAGE_NAME>:latest` |
| View container logs | `docker logs ingres-client-env` |
| Export loaded image to tar | `docker save <IMAGE_NAME>:latest -o <IMAGE_NAME>.tar` |

---

## Actian Ingres Client – install paths inside the container

| Path | Purpose |
|---|---|
| `/opt/Actian/Actian_Client` | `II_SYSTEM` – installation root |
| `/opt/Actian/Actian_Client/ingres/bin` | Client binaries |
| `/opt/Actian/Actian_Client/ingres/utility` | Utility programs (`netutil`, `iinamu`, etc.) |
| `/opt/Actian/Actian_Client/ingres/files` | Configuration files (`odbc.ini`, etc.) |

---

## Troubleshooting

### Port 8090 is already in use
Use a different host-side port:
```bash
docker run -d --name ingres-client-env -p 9090:80 <IMAGE_NAME>:latest
```

### Container exits immediately
```bash
docker logs ingres-client-env
```

### Apache not responding
```bash
docker ps
curl http://localhost:8090
```

### Docker Desktop on Windows – WSL 2 required
```powershell
wsl --install
```
Then restart Docker Desktop.

### `docker compose` command not found
Your system may have Docker Compose v1 (standalone). Use `docker-compose` instead of `docker compose` in all commands.

### Build fails during unixODBC compile step *(odbc2.3.12 only)*
Ensure the build machine has internet access – the unixODBC 2.3.12 source is downloaded automatically from GitHub during the build.

### ODBC connection failure
1. Confirm `ingstart` ran successfully inside the container.
2. Verify the vnode is correctly configured with `netutil` inside the container.
3. Check the DSN entry in `/opt/Actian/Actian_Client/ingres/files/odbc.ini`.
4. Review the ODBC trace log: `tail -f /var/www/html/logs/odbc_trace.log`

---

*For support, provide the output of `docker logs ingres-client-env` and any relevant error messages to your Actian representative.*
