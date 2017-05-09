library(shiny)
library(quantmod)
library(highcharter)
library(rhandsontable)

shinyServer(function(input, output) {
    observeEvent(input$go, {
        print("Hello, World!")
    })
})