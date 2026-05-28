#!/bin/bash
set -e

mkdir -p /var/run/apache2 /var/www/html/logs
chown www-data:www-data /var/www/html/logs 2>/dev/null || true

echo "Starting Apache2..."
apache2ctl start || true

echo "Starting Actian Ingres Client network layer..."
source /etc/environment 2>/dev/null || true
export II_SYSTEM=/opt/Actian/Actian_Client
export PATH=$II_SYSTEM/ingres/bin:$II_SYSTEM/ingres/utility:$PATH
su - actian -c "
    export II_SYSTEM=/opt/Actian/Actian_Client
    export PATH=\$II_SYSTEM/ingres/bin:\$II_SYSTEM/ingres/utility:\$PATH
    ingstart
" || echo "WARNING: ingstart failed – start it manually as the actian user."

echo "Container ready."
echo "  Restart Apache : sudo apache2ctl restart   (or: sudo service apache2 restart)"
echo "  Shell access   : docker exec -it <container> bash"

# Keep container alive; bash wait loop reaps zombie children (PID 1 duty)
while true; do
    wait 2>/dev/null || true
    sleep 1
done
