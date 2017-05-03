
# Dependencies

install_if_not_present <- function(name) {
    if (!(name %in% rownames(installed.packages()))) {
        install.packages(name)
    }
}

install_if_not_present("shiny")
install_if_not_present("quantmod")
install_if_not_present("highcharter")
install_if_not_present("rhandsontable")