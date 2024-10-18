
# Download the macOS version of butler
curl -L -o butler.zip https://broth.itch.ovh/butler/darwin-amd64/LATEST/archive/default
unzip butler.zip

# Create a local bin directory in your home directory if it doesn't exist
mkdir -p $HOME/bin

# Move the butler binary to $HOME/bin, handling possible subdirectory extraction
if [ -f butler ]; then
    mv butler $HOME/bin/
else
    if [ -f butler-* ]; then
        mv butler-*/butler $HOME/bin/
    else
        echo "Error: 'butler' binary not found."
        exit 1
    fi
fi

# Ensure $HOME/bin is in your PATH
echo 'export PATH=$HOME/bin:$PATH' >> ~/.zshrc
source ~/.zshrc

# Verify the installation
butler -V