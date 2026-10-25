#include <stdio.h>
#include <stdlib.h>

//! @brief Outputs the SSH passphrase stored in an environment variable
//!
//! Reads the `GIT_MANAGER_SSH_PASSPHRASE` environment variable and writes
//! its value to standard output without appending a newline.
//!
//! This program is intended to be used as a non-interactive SSH/Git
//! passphrase helper.
//!
//! @return 0 if the passphrase was successfully written to standard output.
//! @return 1 if the `GIT_MANAGER_SSH_PASSPHRASE` environment variable does not exist.
//! @return 2 if writing the passphrase to standard output fails.
//!
int main(void)
{
    const char *passphrase = getenv("GIT_MANAGER_SSH_PASSPHRASE");

    if (NULL == passphrase)
    {
        return 1;
    }

    if (EOF == fputs(passphrase, stdout))
    {
        return 2;
    }

    return 0;
}
