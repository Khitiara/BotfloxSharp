package br.com.devsrsouza.jda.command

typealias FailExecuteCallback = suspend () -> Unit

class CommandFailException(
    val execute: FailExecuteCallback
) : RuntimeException()