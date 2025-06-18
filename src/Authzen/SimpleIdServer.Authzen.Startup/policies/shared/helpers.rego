package shared.helpers

has_role(role) if {
    input.subject.properties.roles[role]
}