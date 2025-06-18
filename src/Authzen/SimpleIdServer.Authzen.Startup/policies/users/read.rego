package users.read

import data.shared.helpers

default allow := false

allow if {
    helpers.has_role("admin")
}