library(shiny)
library(quantmod)
library(highcharter)
library(rhandsontable)

starting.investment = 100000
start.date = "2017-01-01"

compute.portfolio.daily.market.value <- function(portfolio, dollars) {
    portfolio <- cbind(
        portfolio,
        dollars = portfolio$percentage * dollars)
    df <- NULL

    compute.daily.market.value <- function(symbol, dollars) {

        symbol.data <- getSymbols(
            symbol,
            auto.assign = FALSE,
            from = start.date)

        shares <- dollars / as.numeric(first(Op(symbol.data)))
        market.value <- Ad(symbol.data) * shares

        if (is.null(df)) {
            df <<- data.frame(market.value)
        }
        else {
            df <<- cbind(df, data.frame(market.value))
        }
    }

    mapply(
        compute.daily.market.value,
        portfolio$symbols,
        portfolio$dollars)

    return(cbind(df, data.frame(Total = rowSums(df))))
}

shinyServer(function(input, output) {

    v <- reactiveValues(portfolio = NULL, index = NULL)

    observeEvent(input$go, {

        if (is.null(input$hot)) return()

        # Compute the value of the portfolio and the comparison 
        # portfolio (we do QQQ here)

        portfolio = hot_to_r(input$hot)

        v$portfolio <- compute.portfolio.daily.market.value(
            portfolio,
            starting.investment)
        v$index <- compute.portfolio.daily.market.value(
            data.frame(
                symbols = c("QQQ"),
                percentage = c(1.0),
                stringsAsFactors = FALSE
            ),
            starting.investment
        )
    })

    convert.totals.dataframe.to.xts <- function(df) {
        return(xts(df$Total, as.POSIXct(rownames(df))))
    }

    output$chart <- renderHighchart({
        if (is.null(v$portfolio) || is.null(v$index)) return()
        highchart(type = "stock") %>%
            hc_add_series(
                data = convert.totals.dataframe.to.xts(v$portfolio),
                name = "Portfolio") %>%
            hc_add_series(
                data = convert.totals.dataframe.to.xts(v$index),
                name = "QQQ") %>%
            hc_add_theme(hc_theme_538())
    })

    output$hot <- renderRHandsontable({
        if (!is.null(input$hot)) {
            portfolio = hot_to_r(input$hot)
        } else {
            portfolio = data.frame(
                symbols = c("FB", "AMZN", "NFLX", "GOOG"),
                percentage = c(0.25, 0.25, 0.25, 0.25),
                stringsAsFactors = FALSE)
        }

        rhandsontable(portfolio) %>%
            hot_table(highlightCol = TRUE, highlightRow = TRUE)
    })
})