Source: {PACKAGE_NAME}
Maintainer: {MAINTAINER_NAME} <{MAINTAINER_EMAIL}>
Section: {SECTION}
Priority: {PRIORITY}
Standards-Version: 3.9.2
Build-Depends: debhelper (>= 9)
Homepage: {HOMEPAGE}

Package: {PACKAGE_NAME}
Architecture: {ARCH}
Depends ${{shlib:Depends}}, ${{misc:Depends}}{DEPENDENT_PACKAGES}
Conflicts: {CONFLICT_PACKAGES}
Description: {SHORT_DESCRIPTION}
 {LONG_DESCRIPTION}
