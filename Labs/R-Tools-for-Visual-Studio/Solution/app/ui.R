library(shiny)
library(highcharter)
library(rhandsontable)

shinyUI(
    fluidPage(
        tags$link(rel = "stylesheet", type = "text/css", href = "https://bootswatch.com/paper/bootstrap.css"),
        fluidRow(
            column(width = 3, class = "panel",
                actionButton("go", label = "Plot returns vs. QQQ"),
                rHandsontableOutput("hot")
            ),
            column(width = 9,
                highchartOutput("chart", height = "700px")
            )
        )
    )
)