# xzephir

A lightweight Unix-like operating system built with C# Cosmos framework, featuring a command-line interface, user management system, and built-in text editor.

![xzephir Boot Screen](https://img.shields.io/badge/xzephir-v0.3-blue)
![License](https://img.shields.io/badge/license-GPL%20v3-green)
![Platform](https://img.shields.io/badge/platform-x86-lightgrey)

## Features

- 🔐 **Multi-user system** with secure login and user management
- 👑 **Root privileges** with administrative commands
- 📁 **File system operations** (create, read, delete files and directories)
- ✏️ **Built-in text editor** (MIV - Minimal Interactive Vi-like editor)
- 🖥️ **System information** display with hardware details
- 🎨 **Colorized terminal** with user-specific prompts
- 📂 **Directory navigation** with permission controls

## Quick Start

## Bare Metal
Check the releases tab: you can find the ISO there.

## Build from source

### Prerequisites

- Visual Studio 2019 or later
- Cosmos Development Kit
- VMware Workstation or VirtualBox for testing

### Building

1. Clone the repository
```bash
git clone https://github.com/pixisaur/xzephir.git
cd xzephir
```

2. Open the solution in Visual Studio
3. Build the project (F6)
4. Run in your preferred virtual machine

## Usage

### First Boot

When xzephir boots, you'll see the boot animation followed by the login screen:

```
   ==================================
   |                     __   _     |
   |  __ _____ ___ ___  / /  (_)___ |
   |  \ \ /_ // -_) _ \/ _ \/ / __/ |
   | /_\_\/__/\__/ .__/_//_/_/_/    |
   |            /_/                 |
   |                                |
   ==================================

login or create user
1. login
2. create
choice: 
```

### Creating Your First User

1. Select `2` or type `create`
2. Enter a username
3. Enter a password
4. Your user account will be created with a home directory

### Command Reference

#### Basic Commands
- `help` - Display all available commands
- `whoami` - Show current user and privileges
- `pwd` - Print working directory
- `clear` - Clear the screen
- `logout` - Log out current user
- `shutdown` / `poweroff` - Shut down the system

#### File Operations
- `ls` - List files and directories
- `cd <directory>` - Change directory
- `cat <file>` - Display file contents
- `touch <file>` - Create empty file
- `rm <file>` - Delete file
- `mkdir <directory>` - Create directory
- `rmdir <directory>` - Delete empty directory

#### Text Editing
- `miv` or `edit` - Launch MIV text editor

#### System Information
- `sysinfo` - Display system information including:
  - OS version
  - Screen resolution
  - CPU information
  - RAM amount

#### Administrative Commands (Root Only)
- `users` - List all users
- `grantroot <username>` - Grant root privileges
- `revokeroot <username>` - Revoke root privileges
- `rmuser <username>` - Delete user account

### User Types

#### Regular Users
- Green prompt: `username > `
- Limited to their home directory (`/users/username_home`)
- Cannot access system files or other users' directories

#### Root Users
- Cyan prompt: `[*] username > `
- Can access entire filesystem
- Can manage other users

#### Root Account
- Red prompt: `[!] root > `
- Full system access
- Special warning message on login

## File System Structure

```
0:\
├── users\
│   ├── username.usr          # User credentials (encrypted)
│   ├── username_home\        # User home directory
│   └── root.usr             # Root user credentials
```

## Security Features

- **Password encryption**: User passwords are Base64 encoded
- **Permission system**: Users are restricted to their home directories
- **Root protection**: Root account always maintains admin privileges
- **File access control**: Non-root users cannot access system files

## Development

### Architecture

xzephir is built on the Cosmos framework, which allows C# code to run directly on bare metal. The main components include:

- `Kernel.cs` - Main operating system kernel
- `MIV` - Text editor component
- File system integration using CosmosVFS

### Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Style

- Follow C# naming conventions
- Add comments for complex logic
- Keep methods focused and small
- Handle errors gracefully with user-friendly messages

## System Requirements

### Minimum Requirements
- 32 MB RAM
- x86 processor
- VGA compatible display

### Recommended
- 64 MB RAM or more
- Modern x86 processor
- VMware or VirtualBox for development

## Known Issues

- Directory deletion only works with empty directories
- Limited graphics support (text mode only)
- No network functionality yet
- Single-threaded execution

## Changelog

### v0.3 (Current)
- Multi-user system implementation
- Root privilege system
- File operations (create, read, delete)
- Directory navigation
- Built-in text editor (MIV)
- System information display
- Improved user interface with colors

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [Cosmos Project](https://github.com/CosmosOS/Cosmos) - The framework that makes this possible
- [MIV](https://github.com/bartashevich/MIV) - The text editor, MInimalistic Vi
- Unix/Linux systems - Inspiration for command structure
- The open-source community

## Contact

- Project Link: [https://github.com/pixisaur/xzephir](https://github.com/pixisaur/xzephir)
- Issues: [https://github.com/pixisaur/xzephir/issues](https://github.com/pixisaur/xzephir/issues)

---

*"With great power comes great responsibility!" - linus torvalds* (as quoted in xzephir)
