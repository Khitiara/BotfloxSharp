package khitiara.ffxiv.gearmanagement

import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.runApplication
import org.springframework.boot.web.client.RestTemplateBuilder
import org.springframework.context.annotation.Bean
import org.springframework.web.client.RestTemplate

@SpringBootApplication
class GearManagementApplication {
    @Bean
    fun restTemplate(builder: RestTemplateBuilder): RestTemplate =
        builder.rootUri("https://xivapi.com").build()
}

fun main(args: Array<String>) {
    runApplication<GearManagementApplication>(*args)
}
