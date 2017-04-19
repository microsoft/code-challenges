// DocDB query language mode
var docdbMode = {
    displayName: 'DocumentDB Query Language',
    name: 'DocDB',
    mimeTypes: '',
    fileExtensions: '',

    ignoreCase: true,
    lineComment: '--',

    autoClosingPairs: [
        ['"', '"'], ["'", "'"], ['@brackets']
    ],

    keywords: [
        "as", "asc", "between", "by", "case", "cast", "convert", "cross", "desc", "else", "end", "exists",
        "for", "from", "group", "having", "in", "inner", "insert", "into", "is", "join", "like", "limit",
        "offset", "order", "outer", "over", "select", "set", "then", "top", "update", "value", "when", "where", "with"
    ],

    keywordops: [
        "and", "or", "not"
    ],

    builtinFunctions: [
        // Mathematical Functions
        'abs', 'ceiling', 'exp', 'floor', 'log', 'log10', 'power', 'round', 'sign', 'sqrt', 'square',
        'trunc', 'acos', 'asin', 'atan', 'atn2', 'cos', 'cot', 'degrees', 'pi', 'radians', 'sin', 'tan',

        // Type Checking Functions
        'is_array', 'is_bool', 'is_null', 'is_number', 'is_object', 'is_string', 'is_defined', 'is_primitive',

        // Array Functions
        'array_concat', 'array_contains', 'array_length', 'array_slice',

        // String Functions
        'concat', 'contains', 'endswith', 'index_of', 'left', 'length', 'lower', 'ltrim', 'replace', 'replicate', 'reverse', 'right', 'rtrim', 'startswith', 'substring', 'upper'
    ],

    builtins: [
        "root", "number", "string", "array", "object"
    ],

    operators: [
        "=", "<", ">", "<=", ">=", "!=", "<>", "+", "-", "*", "/", "%", "||", "|", "&", "^"
    ],

    brackets: [
        { open: '(', close: ')', token: 'delimiter.parenthesis' },
        { open: '{', close: '}', token: 'delimiter.curly' },
        { open: '[', close: ']', token: 'delimiter.square' }
    ],

    decpart: /\d(_?\d)*/,
    decimal: /0|@decpart/,

    // Common regular expressions
    symbols: /[=><!~?:&|+\-*\/\^%\.]+/,

    // The main tokenizer for our languages
    tokenizer: {
        root: [
            // Identifiers and keywords
            [/[a-zA-Z_][\w_]*/, {
                cases: {
                    "true|false|null|undefined": "predefined",
                    "@keywords": "keyword",
                    "@keywordops": "keyword",
                    "@builtinFunctions": "ext-docdb-builtinFunction",
                    "@builtins": "predefined",
                    "@default": ""
                }
            }],

            // Whitespace
            { include: "@whitespace" },

            // Strings
            [/"/, 'string', '@dstring.\"'],
            [/'/, 'string', '@sstring.\''],

            // Operators
            [/[{}()\[\]]/, "@brackets"],
            [/@symbols/, {
                cases: {
                    "@operators": "operator",
                    "@default": ""
                }
            }],

            // Numbers
            [/0[xX][0-9a-fA-F](_?[0-9a-fA-F])*/, 'number.hex'],
            [/0[dD]@decpart/, 'number'],
            [/@decimal((\.@decpart)?([eE][\-+]?@decpart)?)/, {
                cases: {
                    '$1': 'number.float',
                    '@default': 'number'
                }
            }],


            // Delimiter: after number because of .\d floats
            [/[,.]/, "delimiter"],

        ],

        // Single quote strings (also used for symbols)
        // sstring.<kind>  where kind is 'sq' (single quote) or 's' (symbol)
        sstring: [
            [/[^\\']+/, 'string'],
            [/\\./, 'string.invalid'],
            [/'/, {
                cases: {
                    '$#==$S2': { token: 'string', bracket: '@close', next: '@pop' },
                    '@default': 'string'
                }
            }]
        ],

        // Double quoted "string". 
        // dstring.<kind>.<delim> where kind is 'd' (double quoted), 'x' (command), or 's' (symbol)
        // and delim is the ending delimiter (" or `)
        dstring: [
            [/[^\\"]+/, 'string'],
            [/\\./, 'string.invalid'],
            [/"/, {
                cases: {
                    '$#==$S2': { token: 'string', bracket: '@close', next: '@pop' },
                    '@default': 'string'
                }
            }]
        ],

        whitespace: [
            [/[\s]+/, 'white'],
            [/--.*$/, 'comment'],
        ],
    }
};
require.config({
    baseUrl: "../Scripts"
});

var editor,
    resultHighlight;

$(function () {
    async(initializeQueryEditor, null);
    async(initializeResultEditor, null);
});

// Reset button
$(".btn-reset").on("click", function () {
    editor.setValue('');
});

function showLoading() {
    $("#results-pane").css("text-align", "center");
    $(".centerer").css("display", "inline-block");
    $(".centered").css("display", "inline-block");
}

function hideLoading() {
    $("#results-pane").css("text-align", "left");
    $(".centerer").css("display", "none");
    $(".centered").css("display", "none");
}

// Run button
var docs = null;
var error = null;
$(".btn-run").on("click", function () {
    var query = editor.getValue();

    if (query === "") {
        $(".result-pager").addClass("hidden");
        setErrorPaneWithAnimation(JSON.stringify({ "errors": [{ "severity": "Error", "location": { "start": 0, "end": 0 }, "code": "SC1002", "message": "Syntax error, unexpected end-of-file." }] }, null, 2));
    } else {
        showLoading();
        $.ajax({
            url: "/sql/demo/Home/Query",
            data: {
                query: query
            },
            dataType: "json"
        }).done(function (msg, textStatus, jqXhr) {
            hideLoading();
            if (msg.Error === null) {
                error = null;
                $("#results-pane .monaco-editor").removeClass("error-pane");
                if (msg.Documents.length > 0) {
                    $(".left-arrow").removeClass("disabled-arrow");
                    $(".right-arrow").removeClass("disabled-arrow");
                    docs = msg.Documents;
                    $(".current-page").text(1);
                    $(".left-arrow").addClass("disabled-arrow");
                    $(".total-page").text(docs.length);
                    if (docs.length === 1) {
                        $(".right-arrow").addClass("disabled-arrow");
                    }
                    $(".result-pager").removeClass("hidden");
                    setResultPaneWithAnimation(docs[0]);
                } else {
                    docs = msg.Documents;
                    $(".result-pager").addClass("hidden");
                    $(".current-page").text(0);
                    $(".total-page").text(0);
                    setResultPaneWithAnimation("{ }");
                }
            } else {
                // Hide display, show error
                docs = null;
                $(".result-pager").addClass("hidden");
                error = JSON.stringify(JSON.parse(('{' + msg.Error.substring(0, msg.Error.lastIndexOf('}') + 1) + '}').replace('Message', '"Message"')), null, 2);
                setErrorPaneWithAnimation(error);
            }
        }).fail(function (msg) {
            hideLoading();
            docs = null;
            $(".result-pager").addClass("hidden");
            error = JSON.stringify(JSON.parse(msg.Error || { "errors": [{ "severity": "Error", "location": { "start": 0, "end": 0 }, "code": "SC1002", "message": "Syntax error, unexpected end-of-file." }] }), null, 2);
            setErrorPaneWithAnimation(JSON.stringify(JSON.parse(error), null, 2));
        });
    }
});

// Left arrow
$(".left-arrow").on("click", function () {
    if (parseInt($(".current-page").text()) > 1 && docs !== null) {
        var currentPage = parseInt($(".current-page").text());
        var prevPage = currentPage - 1;
        // Animate
        setResultPaneWithAnimation(docs[prevPage - 1]);

        $(".current-page").text(prevPage);
        if (prevPage === 1) {
            $(".left-arrow").addClass("disabled-arrow");
        }
        if (currentPage === docs.length) {
            $(".right-arrow").removeClass("disabled-arrow");
        }
    }
})

// Right arrow
$(".right-arrow").on("click", function () {
    if (parseInt($(".current-page").text()) < parseInt($(".total-page").text()) && docs !== null) {
        var currentPage = parseInt($(".current-page").text());
        // Animate
        setResultPaneWithAnimation(docs[currentPage]);

        $(".current-page").text(currentPage + 1);
        if (currentPage === 1) {
            $(".left-arrow").removeClass("disabled-arrow");
        }
        if ((currentPage + 1) >= docs.length) {
            $(".right-arrow").addClass("disabled-arrow");
        }
    }
})

function setErrorPaneWithAnimation(result) {
    $(".results-container").fadeOut(300, function () {
        resultHighlight.setValue(result);
        $("#results-pane .monaco-editor").addClass("error-pane");
        $(".results-container").fadeIn(300, function () {
            $("#results-pane .monaco-editor").addClass("error-pane");
        });
    });
}

function setResultPaneWithAnimation(result) {
    $(".results-container").fadeOut(300, function () {
        resultHighlight.setValue(result);
        $(".results-container").fadeIn(300);
    });
}

function setResultPaneWithoutAnimation(result) {
    resultHighlight.setValue(result);
}

function initializeQueryEditor() {
    var container = document.getElementById("vsEditor");

    require(["vs/editor/editor.main"], function () {
        editor = Monaco.Editor.create(container, {
            mode: docdbMode,
            wrappingColumn: 0,
            scrollBeyondLastLine: false,
            automaticLayout: true
        });
    });
}

function initializeResultEditor() {
    var resultContainer = document.getElementById("results-pane");

    require(["vs/editor/editor.main"], function () {
        resultHighlight = Monaco.Editor.create(resultContainer, {
            mode: "application/json",
            lineNumbers: false,
            wrappingColumn: 0,
            scrollBeyondLastLine: false,
            readOnly: true,
            roundedSelection: false,
            automaticLayout: true,
            scrollbar: {
                // other values are "visible" or "hidden"
                vertical: "auto",
                horizontal: "hidden"
            },
            value: ""
        });
    });
}

function async(func, callback) {
    setTimeout(function () {
        func();
        if (callback) { callback(); }
    }, 0);
}

String.prototype.capitalizeFirstLetter = function () {
    return this.charAt(0).toUpperCase() + this.slice(1);
};
String.format = function () {
    var s = arguments[0];
    for (var i = 0; i < arguments.length - 1; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        s = s.replace(reg, arguments[i + 1]);
    }
    return s;
};
